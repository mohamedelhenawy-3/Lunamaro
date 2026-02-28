namespace Lunamaroapi.DTOs.DiscountTierDto
{
    public class UpdateDiscountTierDTO
    {
        public decimal MinimumAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
