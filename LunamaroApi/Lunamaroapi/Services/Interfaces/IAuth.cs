using Lunamaroapi.DTOs.userDTO;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IAuth
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);

    }
}
