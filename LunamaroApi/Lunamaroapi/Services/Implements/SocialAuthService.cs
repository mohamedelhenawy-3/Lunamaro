using Google.Apis.Auth;
using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;

namespace Lunamaroapi.Services.Implements
{
    public class SocialAuthService : ISocialAuthService
    {

        private readonly IConfiguration _config;

        public SocialAuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<SocialUserInfo?> VerifyTokenAsync(string token, string provider)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(provider))
                return null;

            SocialAuthResult result;

            if (provider.Equals("GOOGLE", StringComparison.OrdinalIgnoreCase))
            {
                result = await VerifyGoogleToken(token);
            }
            else if (provider.Equals("FACEBOOK", StringComparison.OrdinalIgnoreCase))
            {
                result = await VerifyFacebookToken(token);
            }
            else
            {
                return null;
            }

            if (!result.Success) return null;

            return new SocialUserInfo
            {
                Email = result.Email ?? "",
                Name = result.Name ?? "",
                Provider = provider
            };
        }
        private async Task<SocialAuthResult> VerifyFacebookToken(string accessToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync(
                    $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}");

                if (!response.IsSuccessStatusCode)
                {
                    return new SocialAuthResult
                    {
                        Success = false,
                        Message = "Invalid Facebook token"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();

                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                return new SocialAuthResult
                {
                    Success = true,
                    Email = data.email,
                    Name = data.name
                };
            }
            catch (Exception ex)
            {
                return new SocialAuthResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        private async Task<SocialAuthResult> VerifyGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _config["Authentication:Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new SocialAuthResult
                {
                    Success = true,
                    Email = payload.Email,
                    Name = payload.Name
                };
            }
            catch (Exception ex)
            {
                return new SocialAuthResult { Success = false, Message = ex.Message };
            }
        }

    }
}
