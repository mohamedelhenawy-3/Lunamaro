using Lunamaroapi.Models;
using Lunamaroapi.Models.Offers;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IPricingService
    {
        Task<PricingResult> CalculateAsync(List<UserCart> cartItems);
        //Task<decimal> GetTierDiscount(decimal subtotal);
        //Task<int?> GetEligibleReward(decimal total);
    }
}
