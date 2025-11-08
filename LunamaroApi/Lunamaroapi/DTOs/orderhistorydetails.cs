using Lunamaroapi.Models;
using System.ComponentModel.DataAnnotations;

namespace Lunamaroapi.DTOs
{
    public class orderhistorydetails
    {
        public int OrderId { get; set; }
        public DateTime DateOfOrder { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public double TotalAmount { get; set; }

  
        public List<OrderitemshistoryDTO> orderItems { get; set; }
    }
}
