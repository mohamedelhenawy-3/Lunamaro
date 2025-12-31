using Lunamaroapi.Models;
using Microsoft.AspNetCore.Identity;

namespace Lunamaroapi.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetUserByEmail(string Email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
