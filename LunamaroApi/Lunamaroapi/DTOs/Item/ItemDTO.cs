namespace Lunamaroapi.DTOs.Item
{
    public class ItemDTO
    {

        
            public int Id { get; set; }          // ❌ ignore
            public string Name { get; set; }     // ✅ required
            public string Description { get; set; } // ✅ required
            public decimal Price { get; set; }    // ✅ required
            public int quantity { get; set; }    // ✅ required
            public int CategoryId { get; set; }  // ✅ required
        public IFormFile? File { get; set; }  // Image file
        public string? ImageUrl { get; set; }


    }
}
