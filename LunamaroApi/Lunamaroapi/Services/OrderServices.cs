using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Lunamaroapi.Services
{
    public class OrderServices : IOrder
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderServices(AppDBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        public async Task<OrderDto?> OrderDone(OrderDto orderDto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            var currentUser = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (currentUser == null)
                return null;

            // ✅ Load cart
            var userCart = await _db.UserCarts
                .Include(c => c.Item)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (userCart == null || !userCart.Any())
                return null;

            double total = userCart.Sum(c => (double)c.Item.Price * c.Quantity);
            if (total <= 0)
                return null;

            // ✅ Create Order
            var orderHeader = new UserOrderHeader
            {
                UserId = userId,
                DateOfOrder = DateTime.Now,
                TotalAmount = total,
                PhoneNumber = orderDto.UserOrderHeader.PhoneNumber,
                DeliveryStreetAddress = orderDto.UserOrderHeader.DeliveryStreetAddress,
                City = orderDto.UserOrderHeader.City,
                State = orderDto.UserOrderHeader.State,
                PostalCode = orderDto.UserOrderHeader.PostalCode,
                Name = orderDto.UserOrderHeader.Name,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = "Not Paid",
                OrderItems = new List<OrderItem>()
            };

            // ✅ Add items to Order
            foreach (var cart in userCart)
            {
                orderHeader.OrderItems.Add(new OrderItem
                {
                    ItemId = cart.ItemId,
                    Quantity = cart.Quantity,
                    UnitPrice = (decimal)cart.Item.Price
                });
            }

            // ✅ Save order
            await _db.UserOrderHeaders.AddAsync(orderHeader);
            await _db.SaveChangesAsync();

            // ✅ Stripe Session
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"https://localhost:4200/payment-success/{orderHeader.Id}",
                CancelUrl = $"https://localhost:4200/payment-failed/{orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>()
            };

            foreach (var cart in userCart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    Quantity = cart.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(cart.Item.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Item.Name
                        }
                    }
                });
            }

            var sessionService = new SessionService();
            Session session = await sessionService.CreateAsync(options);

            // ✅ Save Stripe info
            orderHeader.StripeSessionId = session.Id;
            orderHeader.StripePaymentIntentId = session.PaymentIntentId;

            _db.UserOrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();

            // ✅ Return URL to Angular
            return new OrderDto
            {
                UserOrderHeader = orderHeader,
                UserCartList = userCart,
                StripeUrl = session.Url
            };
        }
        public async Task<bool> OrderSuccess(string sessionId)
        {
            var order = await _db.UserOrderHeaders
                .FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);

            if (order == null)
                return false;

            order.PaymentStatus = "Paid";
            order.OrderStatus = OrderStatus.Processing;
            order.PaymentProcessDate = DateTime.Now;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OrderCancel(string sessionId)
        {
            var order = await _db.UserOrderHeaders
                .FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);

            if (order == null)
                return false;

            order.PaymentStatus = "Cancelled";
            order.OrderStatus = OrderStatus.Cancelled;

            await _db.SaveChangesAsync();
            return true;
        }


    }
}
