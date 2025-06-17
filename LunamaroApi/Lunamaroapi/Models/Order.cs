namespace Lunamaroapi.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public double TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
