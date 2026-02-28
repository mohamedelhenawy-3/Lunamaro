using Lunamaroapi.Data;
using Lunamaroapi.Models.Offers;
using Lunamaroapi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Repositories.Implementations
{
    public class OfferRepository : IOffersRepository
    {
        private readonly AppDBContext _context;

        public OfferRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountTier>> GetDiscountTiersAsync()
        {
            return await _context.DiscountTiers.ToListAsync();
        }

        public async Task<List<WeeklyDeal>> GetActiveWeeklyDealsAsync()
        {
            return await _context.WeeklyDeals
                .Where(d => d.IsActive && d.ExpiryDate > DateTime.Now)
                .ToListAsync();
        }

        public async Task<List<AddOnReward>> GetActiveRewardsAsync()
        {
            return await _context.AddOnRewards
                .Where(r => r.IsActive)
                .ToListAsync();
        }
    }
}
