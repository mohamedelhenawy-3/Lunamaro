using Lunamaroapi.Models;
using Lunamaroapi.Models.Offers;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;

namespace Lunamaroapi.Services.Implements
{
    public class PricingService : IPricingService
    {
        private readonly IOffersRepository _offerRepository;

        public PricingService(IOffersRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<PricingResult> CalculateAsync(List<UserCart> cartItems)
        {
            var deals = await _offerRepository.GetActiveWeeklyDealsAsync();
            var tiers = await _offerRepository.GetDiscountTiersAsync();
            var rewards = await _offerRepository.GetActiveRewardsAsync();

            decimal originalTotal = 0;
            decimal offerDiscount = 0;

            foreach (var item in cartItems)
            {
                decimal unitPrice = item.Item.Price;
                decimal itemOriginal = unitPrice * item.Quantity;

                originalTotal += itemOriginal;

                var deal = deals.FirstOrDefault(d => d.ProductId == item.ItemId);

                if (deal != null)
                {
                    decimal discountPerUnit = unitPrice * deal.DiscountPercentage / 100;
                    offerDiscount += discountPerUnit * item.Quantity;
                }
            }

            decimal afterOfferTotal = originalTotal - offerDiscount;

            decimal tierDiscount = GetTierDiscount(afterOfferTotal, tiers);

            decimal finalTotal = afterOfferTotal - tierDiscount;

            int? freeProductId = GetEligibleReward(finalTotal, rewards);

            return new PricingResult
            {
                OriginalTotal = originalTotal,
                OfferDiscount = offerDiscount,
                TierDiscount = tierDiscount,
                FinalTotal = finalTotal,
                FreeProductId = freeProductId
            };
        }











        //private methods fr calculation
        private decimal ApplyWeeklyDeals(List<UserCart> items, List<WeeklyDeal> deals)
        {
            decimal subtotal = 0;

            foreach (var item in items)
            {
                var deal = deals.FirstOrDefault(d => d.ProductId == item.ItemId);

                decimal price = (decimal)item.Item.Price;

                if (deal != null)
                {
                    price -= price * deal.DiscountPercentage / 100;
                }

                subtotal += price * item.Quantity;
            }

            return subtotal;
        }
        private decimal GetTierDiscount(decimal subtotal, List<DiscountTier> tiers)
        {
            var orderedTiers = tiers
                .OrderByDescending(t => t.MinimumAmount)
                .ToList();

            foreach (var tier in orderedTiers)
            {
                if (subtotal >= tier.MinimumAmount)
                    return tier.DiscountAmount;
            }

            return 0;
        }
        private int? GetEligibleReward(decimal total, List<AddOnReward> rewards)
        {
            var orderedRewards = rewards
                .OrderByDescending(r => r.MinimumAmount)
                .ToList();

            foreach (var reward in orderedRewards)
            {
                if (total >= reward.MinimumAmount)
                    return reward.FreeProductId;
            }

            return null;
        }
    }
}
