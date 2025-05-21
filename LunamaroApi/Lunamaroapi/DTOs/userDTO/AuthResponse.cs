namespace Lunamaroapi.DTOs.userDTO
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public List<string>? Errors { get; set; }
    }
}
