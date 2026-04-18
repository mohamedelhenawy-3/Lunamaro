namespace Lunamaroapi.Services.Interfaces
{
    public interface IOrderNotificationService
    {
        Task SendOrderPlacedAsync(UserOrderHeader order);
        Task SendOutForDeliveryAsync(UserOrderHeader order);
    }
}
