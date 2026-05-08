namespace Lunamaroapi.DTOs.UserCart
{
    public class AddToCartDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public List<int> AddOnIds { get; set; } = new();
    }
}
