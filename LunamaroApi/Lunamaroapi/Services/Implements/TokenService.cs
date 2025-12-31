using Azure.Core;
using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.Helper;
using Lunamaroapi.Migrations;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Lunamaroapi.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshToken _refreshTokenRepo;
        private readonly JwtTokenGenerator _jwt;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IRefreshToken refreshTokenRepo, JwtTokenGenerator Jwt)
        {
            _configuration = configuration;
            _userManager = userManager;
            _refreshTokenRepo = refreshTokenRepo;
            _jwt = Jwt;
        }

        public async Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, string deviceId)
        {
            var accessToken = await _jwt.GenerateToken(user);
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(
    Convert.ToDouble(_configuration["JwtSettings:RefreshTokenDays"])
),
                DeviceId = deviceId
            };

            await _refreshTokenRepo.AddAsync(refreshToken);
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

        }

        public async Task LogoutAsync(string refreshTokenValue)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(refreshTokenValue);
            if (token != null)
            {
                token.IsRevoked = true;
                await _refreshTokenRepo.UpdateAsync(token);
            }
        }

        public async  Task<AuthResponseDto?> RefreshTokensAsync(string refreshTokenValue)
        {
            var existingToken = await _refreshTokenRepo.GetByTokenAsync(refreshTokenValue);
            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
                return null;

            await _refreshTokenRepo.RevokeAsync(existingToken);

            // create new access token
            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null) return null;


            var newAccessToken = await _jwt.GenerateToken(user);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                DeviceId = existingToken.DeviceId,
                Token = GenerateRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenDays"]))
            };
            await _refreshTokenRepo.AddAsync(newRefreshToken);
            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

        }

        private string GenerateRandomToken(int size = 64)
        {
            var randomNumber = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
