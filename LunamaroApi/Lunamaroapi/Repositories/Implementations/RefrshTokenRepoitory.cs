using Lunamaroapi.Data;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Repositories.Implementations
{
    public class RefrshTokenRepoitory  : IRefreshToken
    {
        private readonly AppDBContext _db;

        public RefrshTokenRepoitory(AppDBContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _db.RefreshTokens.AddAsync(token);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllByUserAsync(string userId)
        {
            return await _db.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();
 
        
        }

        public async Task<RefreshToken?> GetByTokenAndDeviceAsync(string token, string deviceId)
        {
          return await  _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token && x.DeviceId == deviceId && !x.IsRevoked);
        }

        public  async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token); 
        
        }

        public async Task RevokeAllByUserAsync(string userId)
        {
            var tokens = await _db.RefreshTokens
                           .Where(x => x.UserId == userId && !x.IsRevoked)
                           .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            _db.RefreshTokens.UpdateRange(tokens);
            await _db.SaveChangesAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _db.RefreshTokens.Update(token);

            await _db.SaveChangesAsync();
        
        }
        public async Task UpdateAsync(RefreshToken token)
        {
            _db.RefreshTokens.Update(token); // mark as modified
            await _db.SaveChangesAsync();    // save changes to DB
        }
    }
}
