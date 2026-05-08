using Lunamaroapi.DTOs.Item;
using Stripe;

namespace Lunamaroapi.DTOs.UserCart
{
    public class UserCartV2DTO
    {
        public int ItemId { get; set; }
        public int UserCartId { get; set; }
        public string ItemName { get; set; }
        public decimal price { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }  // For file uploads (optional)
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public List<AddOnDto> AvailableAddOns { get; set; }

        public List<AddOnDto> AddOns { get; set; } = new();

        public decimal TotalPrice { get; set; }
    }  
}
