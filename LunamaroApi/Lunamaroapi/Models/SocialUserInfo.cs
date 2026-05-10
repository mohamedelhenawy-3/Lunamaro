namespace Lunamaroapi.Models
{
    public class SocialUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; // "Google" or "Facebook"
        public string ProviderId { get; set; } = string.Empty;

    }
}
