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
    public class UserCartService : IUserCart
    {

        private readonly AppDBContext _db;


        public UserCartService(AppDBContext db)
        {
            _db = db;
        }

        public async Task AddToCartAsync(string userId, int itemId, int quantity)
        {
            var itemExists = await _db.Items.FindAsync(itemId);
            Console.WriteLine(itemExists);


            var existingCart = await _db.UserCarts
                .FirstOrDefaultAsync(x => x.ItemId == itemId && x.UserId == userId);

            if (existingCart != null)
            {
                existingCart.Quantity += quantity;
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


        public async Task ClearCartAsync(string userId)
        {
            {
                var cartItems = _db.UserCarts.Where(c => c.UserId == userId);
                _db.UserCarts.RemoveRange(cartItems);
                await _db.SaveChangesAsync();
            }
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
