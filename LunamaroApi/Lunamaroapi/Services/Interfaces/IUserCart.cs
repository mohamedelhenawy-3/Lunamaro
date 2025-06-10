using Lunamaroapi.DTOs;
using Lunamaroapi.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IUserCart
    {
        Task AddToCartAsync(string userId, int itemId, int quantity);
        Task<List<UserCartDTO>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(int cartItemId);
        Task UpdateQuantityAsync(int cartItemId, int newQuantity);
        Task<int> GetItemsCartCount(string userId);
        Task ClearCartAsync(string userId);
    }
}
