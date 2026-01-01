using Lunamaroapi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserOrderHeader
{
    public int Id { get; set; }
    public string TemporaryKey { get; set; } = null!;

    // ✅ Not required from client
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }  // ✅ Must be nullable

    [Required]
    public DateTime DateOfOrder { get; set; }

    public DateTime? DateOfShipping { get; set; }

    [Required]
    public double TotalAmount { get; set; }

    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? PaymentProcessDate { get; set; }
    public string? TransactionId { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string DeliveryStreetAddress { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public int PostalCode { get; set; }

    [Required]
    public string Name { get; set; }

    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    public PaymentType paymentType { set; get; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    // ✅ IMPORTANT: initialize to avoid null errors
    public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
}
