using Lunamaroapi.Data;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.DTOs.UserCart;
using Lunamaroapi.Models;
using Lunamaroapi.Models.Cart;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services.Implements
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

        public async Task AddToCartV2(AddToCartDto dto, string userId)
        {
            var cart = new UserCart
            {
                UserId = userId,
                ItemId = dto.ItemId,
                Quantity = dto.Quantity
            };

            _db.UserCarts.Add(cart);
            await _db.SaveChangesAsync();

            if (dto.AddOnIds != null && dto.AddOnIds.Any())
            {
                var addOns = dto.AddOnIds.Select(id => new UserCartAddOn
                {
                    UserCartId = cart.Id,
                    AddOnId = id
                });

                _db.userCartAddOns.AddRange(addOns);
            }

            await _db.SaveChangesAsync(); // 👈 single save at end
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
                    ItemId=c.ItemId,
                    ItemName = c.Item.Name,
                    price = c.Item.Price,
                    Description = c.Item.Description,
                    ImageUrl = c.Item.ImageUrl,
                    Quantity = c.Quantity,
                })
                .ToListAsync();
        }

        public async Task<List<UserCartV2DTO>> GetCartItemsV2(string userId)
        {
            // Query 1 — get cart items with item info and selected addons
            var cartItems = await _db.UserCarts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.ItemId,
                    c.Quantity,
                    ItemName = c.Item.Name,
                    ItemPrice = c.Item.Price,
                    Description = c.Item.Description,
                    ImageUrl = c.Item.ImageUrl,
                    SelectedAddOns = c.AddOns.Select(a => new AddOnDto
                    {
                        Id = a.AddOn.Id,
                        Name = a.AddOn.Name,
                        Price = a.AddOn.Price
                    }).ToList()
                })
                .ToListAsync();

            if (!cartItems.Any())
                return new List<UserCartV2DTO>();

            // Query 2 — get all available addons for all items in ONE query
            var itemIds = cartItems.Select(c => c.ItemId).Distinct().ToList();

            var availableAddOns = await _db.ItemAddOns
                .AsNoTracking()
                .Where(a => itemIds.Contains(a.ItemId))
                .Select(a => new
                {
                    a.ItemId,
                    AddOn = new AddOnDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Price = a.Price
                    }
                })
                .ToListAsync();

            var addOnsByItem = availableAddOns
                .GroupBy(a => a.ItemId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.AddOn).ToList());

            return cartItems.Select(c => new UserCartV2DTO
            {
                UserCartId = c.Id,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                price = c.ItemPrice,
                Quantity = c.Quantity,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                AddOns = c.SelectedAddOns,
                AvailableAddOns = addOnsByItem.GetValueOrDefault(c.ItemId) ?? new List<AddOnDto>(),
                TotalPrice = (c.ItemPrice + c.SelectedAddOns.Sum(a => a.Price)) * c.Quantity
            }).ToList();
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
            var addons = _db.userCartAddOns
                .Where(a => a.UserCartId == cartItemId);

            _db.userCartAddOns.RemoveRange(addons);

            var cartItem = await _db.UserCarts.FindAsync(cartItemId);

            if (cartItem != null)
            {
                _db.UserCarts.Remove(cartItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateCartAddOnsAsync(int userCartId, List<int> addOnIds, string userId)
        {
            var cartItem = await _db.UserCarts
                 .FirstOrDefaultAsync(c => c.Id == userCartId && c.UserId == userId);

            if (cartItem == null) return;
            var existing = _db.userCartAddOns
               .Where(a => a.UserCartId == userCartId);
               _db.userCartAddOns.RemoveRange(existing);

            if (addOnIds.Any())
            {
                var newAddOns = addOnIds.Select(id => new UserCartAddOn
                {
                    UserCartId = userCartId,
                    AddOnId = id
                });
                _db.userCartAddOns.AddRange(newAddOns);
            }

            await _db.SaveChangesAsync();

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
