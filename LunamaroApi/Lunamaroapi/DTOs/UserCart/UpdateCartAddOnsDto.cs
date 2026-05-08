namespace Lunamaroapi.DTOs.UserCart
{
    public class UpdateCartAddOnsDto
    {
        public int UserCartId { get; set; }
        public List<int> AddOnIds { get; set; } = new();
    }
}
