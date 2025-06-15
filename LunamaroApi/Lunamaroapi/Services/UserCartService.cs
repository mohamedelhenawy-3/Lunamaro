using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services
{
    public class UserCartService : 
        IUserCart
    {

        private readonly AppDBContext _db;


        public UserCartService(AppDBContext db)
        {
            _db = db;
        }

        public async Task AddToCartAsync(string userId, int itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var item = await _db.Items.FindAsync(itemId);
            if (item == null)
                throw new ArgumentException("Item not found.");

            if (quantity > item.quantity)
                throw new ArgumentException("Requested quantity exceeds available stock.");

            var existingCart = await _db.UserCarts
                .FirstOrDefaultAsync(x => x.ItemId == itemId && x.UserId == userId);

            if (existingCart != null)
            {
                int newQuantity = existingCart.Quantity + quantity;
                if (newQuantity > item.quantity)
                    throw new ArgumentException("Cannot add more than available stock.");

                existingCart.Quantity = newQuantity;
                _db.UserCarts.Update(existingCart);
            }
            else
            {
                var newCartItem = new UserCart
                {
                    UserId = userId,
                    ItemId = itemId,
                    Quantity = quantity
                };

                await _db.UserCarts.AddAsync(newCartItem);
            }

            await _db.SaveChangesAsync();
        }


        public Task ClearCartAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserCartDTO>> GetCartItemsAsync(string userId)
        {
            return await _db.UserCarts
                .Where(x => x.UserId == userId)
                .Include(x => x.Item)
                .Select(c => new UserCartDTO
                {
                    UserCartId = c.Id,
                    ItemName = c.Item.Name,
                    price = c.Item.Price,
                    Description = c.Item.Description,
                    ImageUrl = c.Item.ImageUrl,
                    Quantity = c.Quantity,
                })
                .ToListAsync();
        }

        public async Task<int> GetItemsCartCount(string userId)
        {
            return await _db.UserCarts
                .Where(c => c.UserId == userId)
                .Select(c => c.ItemId) // or ProductId depending on your model
                .Distinct()
                .CountAsync();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var item = await _db.UserCarts.FindAsync(cartItemId);
            if (item != null)
            {
                _db.UserCarts.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(int cartItemId, int newQuantity)
        {
            var cartItem = await _db.UserCarts.FindAsync(cartItemId);

            if (cartItem != null && newQuantity > 0 && newQuantity <= 10)
            {
                cartItem.Quantity = newQuantity;
                _db.UserCarts.Update(cartItem);
                await _db.SaveChangesAsync();
            }
        }



    }
}
