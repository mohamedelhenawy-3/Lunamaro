namespace Lunamaroapi.DTOs
{
    public class ItemDTO
    {

        public int Id { get; set; }  // Needed in Angular
        public string Name { get; set; } = string.Empty;
            public string Description { get; set; }
            public double Price { get; set; }
             public  int quantity { get; set; }
            public int CategoryId { get; set; }
        public IFormFile? File { get; set; }  // Image file
        public string? ImageUrl { get; set; }


    }
}
