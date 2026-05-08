using Lunamaroapi.DTOs.UserCart;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IUserCart
    {
        Task AddToCartAsync(string userId, int itemId, int quantity);
        Task AddToCartV2(AddToCartDto dto, string userId);

        Task<List<UserCartDTO>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(int cartItemId);
        Task UpdateQuantityAsync(int cartItemId, int newQuantity);
        Task<int> GetItemsCartCount(string userId);
        Task ClearCartAsync(string userId);
        Task UpdateCartAddOnsAsync(int userCartId, List<int> addOnIds, string userId);

        Task<List<UserCartV2DTO>> GetCartItemsV2(string userId);

    }
}
