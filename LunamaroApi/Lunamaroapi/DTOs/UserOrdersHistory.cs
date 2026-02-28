using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs
{
    public class UserOrdersHistory
    {
        public int OrderId { get; set; }
        public DateTime DateOfOrder { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
