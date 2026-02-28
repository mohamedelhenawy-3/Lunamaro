namespace Lunamaroapi.DTOs.Item
{
    public class UpdateItemDTO
    {
        public int Id { get; set; }  // Needed in Angular
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int quantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? File { get; set; }  // Image file
        public string? ImageUrl { get; set; }
        public bool IsSpecial { get; set; }

    }
}
