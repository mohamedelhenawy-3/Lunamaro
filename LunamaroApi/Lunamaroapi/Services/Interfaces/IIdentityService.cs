using Lunamaroapi.DTOs.AuthResponse;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<SocialAuthResult> LoginWithSocialAsync(string provider, string token);
    }
}
