using Lunamaroapi.DTOs;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IOrder
    {
        //Task<object?> GetOrderDetailsAsync(int orderId);
        Task<OrderDetailsDTO> GetOrderPerview(string userId);

        Task<OrderDto> OrderDone(OrderDto orderDto);
        Task<bool> OrderSuccess(string sessionId);
        Task<bool> OrderCancel(string sessionId);

    }
}
