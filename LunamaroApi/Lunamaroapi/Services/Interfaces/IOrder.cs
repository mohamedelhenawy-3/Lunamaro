using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
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
        Task<orderhistorydetails> UserOrderHistoryDetails(int orderId);


        //admin
        Task<orderhistorydetails> OrderHistoryDetailsAd(int orderId);

        Task<IEnumerable<ordersListDTO>> ListOfOrders();
        Task<bool> UpdateStatusAsync(UpdateStatusOrderDTO dto, int orderId);





       
    }
}
