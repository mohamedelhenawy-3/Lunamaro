namespace Lunamaroapi.DTOs.AuthResponse
{
    public class SocialAuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; } // Add this if you want to support it// Your App's final JWT
        public bool IsNewUser { get; set; }
        public string? Email { get; set; }      // From Google/FB
        public string? Name { get; set; }       // From Google/FB

    }
}
