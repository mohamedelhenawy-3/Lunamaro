namespace Lunamaroapi.DTOs
{
    public class CategoryDTO
    {
        public string Name { get; set; } = string.Empty;

     
        public List<ItemDTO>? Items { get; set; }
    }
}
