using Lunamaroapi.Models.Offers;

namespace Lunamaroapi.Repositories.Interfaces
{
    public interface IOffersRepository
    {
        Task<List<DiscountTier>> GetDiscountTiersAsync();
        Task<List<WeeklyDeal>> GetActiveWeeklyDealsAsync();
        Task<List<AddOnReward>> GetActiveRewardsAsync();
    }
}
