using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.DTOs.userDTO;
using Lunamaroapi.Helper;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Lunamaroapi.Services.Implements
{
    public class AuthServices : IAuthServices
    {

        private readonly IAuthRepository _repo;
        private readonly ITokenService _jwtService;
        private readonly ILogger<AuthServices> _loger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshToken _refreshTokenRepo;
        private readonly JwtTokenGenerator _jwt;


        public AuthServices(IAuthRepository repo, ITokenService jwtService, ILogger<AuthServices> Loger, UserManager<ApplicationUser> userManager , IRefreshToken refreshTokenRepo ,JwtTokenGenerator jwt)
        {
            _repo = repo;
            _jwtService = jwtService;
            _loger = Loger;
            _userManager = userManager;
            _refreshTokenRepo = refreshTokenRepo;
            _jwt = jwt;

        }


        public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
        {
            var user = await _repo.GetUserByEmail(request.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            // 2️⃣ Check password
            var validPassword = await _repo.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
                throw new Exception("Invalid credentials");


            var token = await _jwtService.GenerateTokensAsync(user,request.DeviceId);
            _loger.LogInformation("Tokens created successfully for user {Email}", request.Email 
                );


            return  new AuthResponseDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }

        public async Task<SuccessResponseDto> LogoutAsync(string refreshToken, string deviceId)
        {
            var token = await _refreshTokenRepo.GetByTokenAndDeviceAsync(refreshToken, deviceId);
            if (token != null)
            {
                await _refreshTokenRepo.RevokeAsync(token);
            }

            return new SuccessResponseDto { Message = "Logout successful" };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string deviceId)
        {
            var token = await _refreshTokenRepo.GetByTokenAndDeviceAsync(refreshToken, deviceId);
            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(token.UserId);
            if (user == null) throw new Exception("User not found");
            var newAccessToken = await _jwt.GenerateToken(user);
            return new AuthResponseDto { AccessToken = newAccessToken, RefreshToken = token.Token };

        }

        public async Task<SuccessResponseDto> RegisterAsync(RegisterRequest dto)
        {
            var existUser = await _repo.GetUserByEmail(dto.Email);
            if (existUser != null)
                throw new Exception("Email already exists");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName
            };

            var result = await _repo.CreateUserAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            await _userManager.AddToRoleAsync(user, "Customer");

            return new SuccessResponseDto
            {
                Message = "User registered successfully"
            };
        }

    }
}
