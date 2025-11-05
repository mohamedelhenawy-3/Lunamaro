using System.ComponentModel.DataAnnotations.Schema;

namespace Lunamaroapi.Models
{
 public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int UserOrderHeaderId { get; set; }
        [ForeignKey("UserOrderHeaderId")]
        public UserOrderHeader UserOrderHeader { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;

    }

}
