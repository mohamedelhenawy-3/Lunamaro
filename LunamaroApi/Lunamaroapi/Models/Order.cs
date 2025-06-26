using System.ComponentModel.DataAnnotations.Schema;

namespace Lunamaroapi.Models
{
    public class Order
    {
     
            public int OrderId { get; set; }

            public int UserOrderHeaderId { get; set; }
            public UserOrderHeader UserOrderHeader { get; set; }

            public ICollection<OrderItem> OrderItems { get; set; }



    }
}
