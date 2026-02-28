namespace Lunamaroapi.Models.Offers
{
    public class PricingResult
    {
        public decimal OriginalTotal { get; set; }

        public decimal OfferDiscount { get; set; }

        public decimal TierDiscount { get; set; }

        public decimal TotalDiscount =>
            OfferDiscount + TierDiscount;

        public decimal FinalTotal { get; set; }

        public int? FreeProductId { get; set; }
    }
}
