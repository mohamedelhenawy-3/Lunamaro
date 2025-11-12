namespace Lunamaroapi.DTOs.Item
{
    public class ReturnedItemDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public double Price { get; set; }
        public int quantity { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }

    }
}
