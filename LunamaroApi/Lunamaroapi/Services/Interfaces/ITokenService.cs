using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, string deviceId);
        Task<AuthResponseDto?> RefreshTokensAsync(string refreshTokenValue);
        Task LogoutAsync(string refreshTokenValue);
    }
}
