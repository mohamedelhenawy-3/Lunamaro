namespace Lunamaroapi.DTOs.Social
{
    public class SocialLoginRequest
    {
        public string Provider { get; set; } = string.Empty; // "Google" or "Facebook"
        public string Token { get; set; } = string.Empty;
    }
}
