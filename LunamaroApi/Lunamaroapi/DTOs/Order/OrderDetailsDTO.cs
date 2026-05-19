using Lunamaroapi.DTOs.UserCart;
using Lunamaroapi.Models.Offers;

public class OrderDetailsDTO
{
    public int OrderId { get; set; }
    public List<UserCartV2DTO>? UserCartList { get; set; }
    public UserOrderHeader? UserOrderHeader { get; set; }

}
