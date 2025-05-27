namespace Lunamaroapi.DTOs
{
    public class CategoryDTO
    {
   
        public int Id { get; set; }  // ✅ Add this for the frontend to use
       public string Name { get; set; } = string.Empty;
        public List<ItemDTO>? Items { get; set; }

    }
}
