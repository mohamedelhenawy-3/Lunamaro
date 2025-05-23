namespace Lunamaroapi.DTOs
{
    public class ItemDTO
    {
  
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
