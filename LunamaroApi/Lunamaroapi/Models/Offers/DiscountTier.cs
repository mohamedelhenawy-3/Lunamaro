namespace Lunamaroapi.Models.Offers
{
    public class DiscountTier
    {
        public int Id { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsActive { get; set; } = true; // optional, so admin can deactivate

    }
}
