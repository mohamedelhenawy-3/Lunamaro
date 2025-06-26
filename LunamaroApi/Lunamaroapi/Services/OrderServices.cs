using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
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



        public async Task<OrderDetailsDTO> GetOrderPerview(string userId)
        {
            // Get cart items with related Item data
            var userCartList = await _db.UserCarts
                .Include(uc => uc.Item)
                .Where(uc => uc.UserId == userId)
                .Select(uc => new UserCartDTO
                {
                    UserCartId = uc.Id,
                    ItemName = uc.Item.Name,
                    price = uc.Item.Price,
                    Description = uc.Item.Description,
                    ImageUrl = uc.Item.ImageUrl,
                    Quantity = uc.Quantity
                })
                .ToListAsync();

            // ✅ Calculate total cart value
            double totalAmount = userCartList.Sum(c => c.TotalPrice);

            // Get user details
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            // Prepare order header
            var orderHeader = new UserOrderHeader
            {
                UserId = userId,
                DateOfOrder = DateTime.Now,
                TotalAmount = totalAmount,
                PhoneNumber = user?.PhoneNumber ?? "",
                DeliveryStreetAddress = user?.Address ?? "",
                City = user?.City ?? "",
                PostalCode = user?.PostalCode ?? 0,
                Name = user?.FullName ?? ""
            };

            // Return DTO
            var dto = new OrderDetailsDTO
            {
                OrderId = 0,
                UserCartList = userCartList,
                UserOrderHeader = orderHeader
            };

            return dto;
        }


    }
}
