namespace Lunamaroapi.Models.Offers
{
    public class AddOnReward
    {
        public int Id { get; set; }
        public decimal MinimumAmount { get; set; }
        public int FreeProductId { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public Item FreeProduct { get; set; }
    }
}
