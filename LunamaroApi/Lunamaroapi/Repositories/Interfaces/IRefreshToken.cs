using Lunamaroapi.Models;

namespace Lunamaroapi.Repositories.Interfaces
{
    public interface IRefreshToken
    {


            Task<RefreshToken?> GetByTokenAsync(string token);
            Task AddAsync(RefreshToken token);
            Task RevokeAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAndDeviceAsync(string token, string deviceId);
        Task RevokeAllByUserAsync(string userId);
        Task<IEnumerable<RefreshToken>> GetAllByUserAsync(string userId);
           Task UpdateAsync(RefreshToken token);


    }

}

