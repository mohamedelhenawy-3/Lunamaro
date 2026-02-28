namespace Lunamaroapi.DTOs.Order
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string? ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;
    }
}
