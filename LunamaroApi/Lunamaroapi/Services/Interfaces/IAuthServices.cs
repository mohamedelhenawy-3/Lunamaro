

using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.DTOs.userDTO;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDto> LoginAsync(LoginRequest request);
        Task<SuccessResponseDto> RegisterAsync(RegisterRequest request);
        Task<SuccessResponseDto> LogoutAsync(string refreshToken, string deviceId);

        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string deviceId);


    }
}
