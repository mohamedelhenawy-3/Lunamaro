namespace Lunamaroapi.DTOs.AuthResponse
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
