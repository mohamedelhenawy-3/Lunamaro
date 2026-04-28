using Google.Apis.Auth.OAuth2.Web;
using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.Helper;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace Lunamaroapi.Services.Implements
{
    public class IdentityService : IIdentityService
    {

        private readonly ISocialAuthService _socialAuthService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator; // <-- Inject your helper

        public IdentityService(
            ISocialAuthService socialAuthService,
            UserManager<ApplicationUser> userManager,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _socialAuthService = socialAuthService;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<SocialAuthResult> LoginWithSocialAsync(string provider, string token)
        {
            if (_socialAuthService == null) throw new Exception("DEBUG: _socialAuthService is NULL");

            var info = await _socialAuthService.VerifyTokenAsync(token, provider);

            if (info == null) return new SocialAuthResult { Success = false, Message = "DEBUG: Google Token Verification returned NULL info" };

            if (string.IsNullOrEmpty(info.Email)) throw new Exception("DEBUG: Google info returned, but EMAIL is NULL");

            var user = await _userManager.FindByEmailAsync(info.Email);
            bool isNew = false;

            if (user == null)
            {
                isNew = true;
                user = new ApplicationUser
                {
                    Email = info.Email,
                    UserName = info.Email,
                    FullName = info.Name ?? "Google User", // Fallback if Name is null
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return new SocialAuthResult { Success = false, Message = $"DEBUG: User Creation Failed: {errors}" };
                }

                await _userManager.AddToRoleAsync(user, "Customer");
            }

            if (user == null) throw new Exception("DEBUG: User object is NULL before generating token");

            var appToken = await _jwtTokenGenerator.GenerateToken(user);
            return new SocialAuthResult
            {
                Success = true,
               AccessToken = appToken,
                IsNewUser = isNew,
                Email = user.Email,      // Add this
                Name = user.FullName     // Add this };
            };
        }
    }
}
