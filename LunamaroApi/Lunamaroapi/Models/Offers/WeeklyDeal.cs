namespace Lunamaroapi.Models.Offers
{
    public class WeeklyDeal
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Item? Product { get; set; }
    }
}
