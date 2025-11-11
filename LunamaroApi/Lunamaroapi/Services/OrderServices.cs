using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
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

        public async Task<OrderResDTO?> OrderDone(CreateOrderdto dto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            var currentUser = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (currentUser == null)
                return null;

            var userCart = await _db.UserCarts
                .Include(c => c.Item)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!userCart.Any())
                return null;

            double total = userCart.Sum(c => (double)c.Item.Price * c.Quantity);

            var orderHeader = new UserOrderHeader
            {
                UserId = userId,
                DateOfOrder = DateTime.Now,
                TotalAmount = total,
                PhoneNumber = dto.PhoneNumber,
                DeliveryStreetAddress = dto.DeliveryStreetAddress,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Name = dto.Name,
                paymentType = dto.IsPayOnDelivery ? PaymentType.Cash : PaymentType.Visa,

                OrderStatus = OrderStatus.Pending,
                PaymentStatus = dto.IsPayOnDelivery ? "Pending Payment" : "Not Paid" ,
                OrderItems = new List<OrderItem>()
            };

            foreach (var cart in userCart)
            {
                orderHeader.OrderItems.Add(new OrderItem
                {
                    ItemId = cart.ItemId,
                    Quantity = cart.Quantity,
                    UnitPrice = (decimal)cart.Item.Price
                });
            }

            await _db.UserOrderHeaders.AddAsync(orderHeader);
            await _db.SaveChangesAsync();

            if (dto.IsPayOnDelivery)
            {
                _db.UserCarts.RemoveRange(userCart);
                await _db.SaveChangesAsync();

                return new OrderResDTO
                {
                    OrderId = orderHeader.Id,
                    PaymentUrl = null   // No Stripe URL
                };
            }


            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = "http://localhost:4200/payment-success/{CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:4200/payment-failed/{CHECKOUT_SESSION_ID}",
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

            // ✅ Create Stripe session AFTER setting urls
            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(options);

            // ✅ Save generated Stripe IDs
            orderHeader.StripeSessionId = session.Id;
            orderHeader.StripePaymentIntentId = session.PaymentIntentId;
            orderHeader.PaymentStatus = "Paid";
            _db.UserOrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();

            var cartItems = _db.UserCarts.Where(x => x.UserId == userId);
            _db.UserCarts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return new OrderResDTO
            {
                OrderId = orderHeader.Id,
                PaymentUrl = session.Url
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

        public async Task<IEnumerable<UserOrdersHistory>> UserOrderHistory()
        {
            var userId = GetCurrentUserId();



            return await _db.OrderItems.Include(x => x.UserOrderHeader).Where(x => x.UserOrderHeader.UserId == userId).Select(s => new UserOrdersHistory
            {
                OrderId = s.OrderItemId,
                DateOfOrder = s.UserOrderHeader.DateOfOrder,
                OrderStatus = s.UserOrderHeader.OrderStatus,
                TotalAmount = s.UserOrderHeader.TotalAmount

            }).ToListAsync();


               


        }

        public async Task<orderhistorydetails> UserOrderHistoryDetails(int orderId)
        {

            var userId = GetCurrentUserId();
            var order = await _db.UserOrderHeaders
         .Where(x => x.UserId == userId && x.Id == orderId)
         .Include(x => x.OrderItems)
             .ThenInclude(oi => oi.Item)
              // only if you have image list
         .FirstOrDefaultAsync();

            if (order == null)
                return null;

            return new orderhistorydetails
            {
                OrderId = order.Id,
                DateOfOrder = order.DateOfOrder,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                orderItems = order.OrderItems.Select(i => new OrderitemshistoryDTO
                {
                    ProductName = i.Item.Name,
                    ImgUrl = i.Item.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList()
            };
        }

        public async Task<IEnumerable<ordersListDTO>> ListOfOrders()
        {
            var order = await _db.UserOrderHeaders.Include(x => x.OrderItems).Select(x => new ordersListDTO
            {
                OrderId = x.Id,
                CustomerName = x.Name,
                PhoneNumber = x.PhoneNumber,
                totalAmount = x.TotalAmount,

                orderStatus = x.OrderStatus,
                OrderDate = x.DateOfOrder
                ,paymentType=x.paymentType
            }).ToListAsync();


            return order;

        }


        public async Task<bool> UpdateStatusAsync(UpdateStatusOrderDTO dto, int orderId)
        {
            // Find order by id
            var order = await _db.UserOrderHeaders.FindAsync(orderId);
            if (order == null) return false;

            // Update order status
            order.OrderStatus = dto.Status;

            // Optional: if you want to handle Cash on Delivery -> mark Paid when Delivered
            if (order.paymentType == PaymentType.Cash && dto.Status == OrderStatus.Delivered)
            {
                order.PaymentStatus = "Paid";
            }

            // Save changes to database
            await _db.SaveChangesAsync();

            return true;
        }

     

        public async Task<orderhistorydetails> OrderHistoryDetailsAd(int orderId)
        {
            var order = await _db.UserOrderHeaders
                   .Where(x => x.Id == orderId)
                   .Include(x => x.OrderItems)
                       .ThenInclude(oi => oi.Item)
                   .FirstOrDefaultAsync();

            if (order == null)
                return null;

            return new orderhistorydetails
            {
                OrderId = order.Id,
                DateOfOrder = order.DateOfOrder,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                orderItems = order.OrderItems.Select(i => new OrderitemshistoryDTO
                {
                    ProductName = i.Item.Name,
                    ImgUrl = i.Item.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                }).ToList()
            };
        }
    }
}
