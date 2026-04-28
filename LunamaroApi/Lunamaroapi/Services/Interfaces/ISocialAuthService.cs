using Google.Apis.Auth;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface ISocialAuthService
    {
        Task<SocialUserInfo?> VerifyTokenAsync(string token, string provider);

    }
}
