using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs
{
    public class OrderDto
    {
        public IEnumerable<UserCart>? UserCartList { get; set; }
        public UserOrderHeader? UserOrderHeader { get; set; }
        public string? StripeUrl { get; set; }

    }
}
