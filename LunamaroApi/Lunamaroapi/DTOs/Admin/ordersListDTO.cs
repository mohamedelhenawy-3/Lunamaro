using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.Admin
{
    public class ordersListDTO
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public double totalAmount { get; set; }
        public OrderStatus orderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public PaymentType paymentType { get; set; }
    }
}
