using Lunamaroapi.DTOs;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IOrder
    {
        //Task<object?> GetOrderDetailsAsync(int orderId);
        Task<OrderDetailsDTO> GetOrderPerview(string userId);
        Task<OrderResDTO?> OrderDone(CreateOrderdto dto);
        Task<bool> OrderSuccess(string sessionId);
        Task<bool> OrderCancel(string sessionId);



        Task<IEnumerable<UserOrdersHistory>> UserOrderHistory();

    }
}
