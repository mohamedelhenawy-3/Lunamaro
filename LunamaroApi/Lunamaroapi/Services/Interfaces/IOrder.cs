using Lunamaroapi.DTOs;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IOrder
    {
        //Task<object?> GetOrderDetailsAsync(int orderId);
        Task<OrderDetailsDTO> GetOrderPerview(string userId);


    }
}
