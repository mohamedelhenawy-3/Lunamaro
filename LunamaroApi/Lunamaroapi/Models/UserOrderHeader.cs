using Lunamaroapi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserOrderHeader
{
    public int Id { get; set; }

    public string TemporaryKey { get; set; } = null!;

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [Required]
    public DateTime DateOfOrder { get; set; }

    public DateTime? DateOfShipping { get; set; }

    // 💰 Financial Breakdown

    [Required]
    public decimal OriginalTotalAmount { get; set; }      // Before ANY discount

    public decimal OfferDiscountAmount { get; set; }      // Product offers

    public decimal TierDiscountAmount { get; set; }       // Tier/global discount

    public decimal TotalDiscountAmount { get; set; }      // Offer + Tier

    [Required]
    public decimal FinalTotalAmount { get; set; }         // Final price user pays


    // 💳 Payment
    public string? StripeSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? PaymentProcessDate { get; set; }
    public string? TransactionId { get; set; }

    // 📦 Shipping Info
    [Required]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string DeliveryStreetAddress { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string State { get; set; } = null!;

    [Required]
    public int PostalCode { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    public PaymentType PaymentType { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}