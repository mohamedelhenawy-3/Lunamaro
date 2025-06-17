namespace Lunamaroapi.DTOs
{
    public class OrderDetailsDTO
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}
