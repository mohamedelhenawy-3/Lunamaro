namespace Lunamaroapi.DTOs.AddOnRewardDto
{
    public class CreateAddOnRewardDTO
    {
        public decimal MinimumAmount { get; set; }
        public int FreeProductId { get; set; }
        public bool IsActive { get; set; }
    }
}
