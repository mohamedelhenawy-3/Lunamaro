namespace Lunamaroapi.DTOs.UserCart
{
    public class UserCartDTO
    {
        public int UserCartId { get; set; }
        public string ItemName { get; set; }
        public double price { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }  // For file uploads (optional)
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }

        public double TotalPrice => price * Quantity;
    }
}

