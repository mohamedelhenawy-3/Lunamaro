namespace Lunamaroapi.DTOs.DiscountTierDto
{
    public class CreateDiscountTierDTO
    {
        public decimal MinimumAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
