using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services
{
    public class OrderServices : IOrder
    {
        private readonly AppDBContext _db;

        public OrderServices(AppDBContext db)
        {
            _db = db;
        }

        public async Task<List<OrderItemDTO>> GetCartPerview(string userId)
        {
            // Get the user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return null;

            // Get user's cart with item info
            var userCart = await _db.UserCarts
                .Include(uc => uc.Item)
                .Where(uc => uc.UserId == userId)
                .ToListAsync();

            if (userCart == null || !userCart.Any())
                return null;

            var dtoList = userCart.Select(uc => new OrderItemDTO
            {
                OrderItemId = 0,
                ItemId = uc.ItemId,
                ItemName = uc.Item.Name,
                ImageUrl = uc.Item.ImageUrl,
                Description = uc.Item.Description,
                UnitPrice = uc.Item.Price,
                Quantity = uc.Quantity
            }).ToList();

            return dtoList;
           

        }

        public async Task<OrderDetailsDTO> GetOrderPerview(string userId)
        {
            // Get the user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return null;

            // Get user's cart with item info
            var userCart = await _db.UserCarts
                .Include(uc => uc.Item)
                .Where(uc => uc.UserId == userId)
                .ToListAsync();

            if (userCart == null || !userCart.Any())
                return null;

            // Calculate total price
            double totalPrice = userCart.Sum(uc => uc.Item.Price * uc.Quantity);

            // Build the DTO
            var dto = new OrderDetailsDTO
            {
                OrderId = 0, // This is a preview, no actual order placed
                FullName = user.FullName,
                Email = user.Email,
                ShippingAddress = "To be filled during checkout",
                OrderDate = DateTime.Now,
                TotalAmount = totalPrice,
                Items = userCart.Select(uc => new OrderItemDTO
                {
                    OrderItemId = 0, // No real ID yet
                    ItemId = uc.ItemId,
                    ItemName = uc.Item.Name,
                    ImageUrl = uc.Item.ImageUrl,
                    Description = uc.Item.Description,
                    UnitPrice = uc.Item.Price,
                    Quantity = uc.Quantity
                }).ToList()
            };

            return dto;
        }

    }
}
