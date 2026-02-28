using Lunamaroapi.DTOs.UserCart;
using Lunamaroapi.Models.Offers;

public class OrderDetailsDTO
{
    public int OrderId { get; set; }
    public List<UserCartDTO>? UserCartList { get; set; }
    public UserOrderHeader? UserOrderHeader { get; set; }

}
