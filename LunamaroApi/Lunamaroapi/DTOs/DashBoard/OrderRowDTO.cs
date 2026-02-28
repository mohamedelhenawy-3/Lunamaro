using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.DashBoard
{
    public class OrderRowDTO
    {
        public int OrderId { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Date { get; set; }
    }
}
