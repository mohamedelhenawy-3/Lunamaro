namespace Lunamaroapi.DTOs.WeaklyDto
{
    public class UpdateWeeklyDealDTO
    {
        public int ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
