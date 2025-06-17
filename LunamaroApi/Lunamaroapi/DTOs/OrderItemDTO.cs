namespace Lunamaroapi.DTOs
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string? ImageUrl { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }

        public double Total => UnitPrice * Quantity;
    }
}
