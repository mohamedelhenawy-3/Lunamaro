using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
using Lunamaroapi.DTOs.Order;
using Lunamaroapi.Services.Interfaces;
using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
using Lunamaroapi.DTOs.Order;
using Lunamaroapi.DTOs.UserCart;
using Lunamaroapi.Helper;
using Lunamaroapi.Helper.ServiceResult;
using Lunamaroapi.Models;
using Lunamaroapi.Queues;
using Lunamaroapi.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
namespace Lunamaroapi.Services.Implements
{
    public delegate Task OrderPlacceHandler(UserOrderHeader order);
    public delegate Task OrderStatusChangedHandler(UserOrderHeader order);
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;

        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderService> __Loger;
        public event OrderPlacceHandler? OnOrderPlaced;
        private readonly IOrderNotificationService _notificationService;
        public event OrderStatusChangedHandler? OnOrderStatusChanged;
        private readonly IPricingService _pricingService;
        public OrderService(
            IConfiguration c,
       AppDBContext db,
       IHttpContextAccessor httpContextAccessor,
       IOrderNotificationService notificationService,
       ILogger<OrderService> logger,
       IPricingService pricingService)
        {
            _configuration = c;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService; 
            OnOrderPlaced += _notificationService.SendOrderPlacedAsync;
            __Loger = logger;
            _pricingService = pricingService;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public Task<object?> GetOrderDetailsAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public async  Task<OrderDetailsDTO> GetOrderPerview(string userId)
        {
            var userCart = await _db.UserCarts
        .Include(uc => uc.Item)
        .Where(uc => uc.UserId == userId)
        .ToListAsync();

            var userCartList = userCart.Select(uc => new UserCartDTO
            {
                UserCartId = uc.Id,
                ItemId = uc.ItemId,
                ItemName = uc.Item.Name,
                price = uc.Item.Price,
                Description = uc.Item.Description,
                ImageUrl = uc.Item.ImageUrl,
                Quantity = uc.Quantity
            }).ToList();

            var pricingResult = await _pricingService.CalculateAsync(userCart);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var orderHeader = new UserOrderHeader
            {
                UserId = userId,
                DateOfOrder = DateTime.UtcNow,
                OriginalTotalAmount = pricingResult.OriginalTotal,
                OfferDiscountAmount = pricingResult.OfferDiscount,
                TierDiscountAmount = pricingResult.TierDiscount,
                TotalDiscountAmount = pricingResult.TotalDiscount,
                FinalTotalAmount = pricingResult.FinalTotal,
                PhoneNumber = user?.PhoneNumber ?? "",
                DeliveryStreetAddress = user?.Address ?? "",
                City = user?.City ?? "",
                PostalCode = user?.PostalCode ?? 0,
                Name = user?.FullName ?? ""
            };

            return new OrderDetailsDTO
            {
                OrderId = 0,
                UserCartList = userCartList,
                UserOrderHeader = orderHeader,
            };
        }        

        public async  Task<IEnumerable<ordersListDTO>> ListOfOrders()
        {
            var order = await _db.UserOrderHeaders.Include(x => x.OrderItems).Select(x => new ordersListDTO
            {
                OrderId = x.Id,
                CustomerName = x.Name,
                PhoneNumber = x.PhoneNumber,
                totalAmount = x.FinalTotalAmount,

                orderStatus = x.OrderStatus,
                OrderDate = x.DateOfOrder
                            ,
                paymentType = x.PaymentType
            }).ToListAsync();


            return order;
        }

        public  async Task<bool> OrderCancel(string sessionId)
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

        public async Task<OrderResDTO?> OrderDone(CreateOrderdto dto)
        {
            var userId = GetCurrentUserId();
            var requestId = Guid.NewGuid().ToString().Substring(0, 8);

            if (string.IsNullOrEmpty(userId)) return null;

            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            var keyUsed = await _db.UserOrderHeaders.AnyAsync(o => o.TemporaryKey == dto.TemporaryKey);
            if (keyUsed) return null;

            var userCart = await _db.UserCarts.Include(c => c.Item).Where(c => c.UserId == userId).ToListAsync();
            if (!userCart.Any()) return null;

            // ✅ THIS IS THE FIX
            var executionStrategy = _db.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var pricingResult = await _pricingService.CalculateAsync(userCart);

                    foreach (var cart in userCart)
                    {
                        var affected = await _db.Database.ExecuteSqlRawAsync(
                            @"UPDATE Items SET Quantity = Quantity - @qty WHERE Id = @itemId AND Quantity >= @qty",
                            new SqlParameter("@qty", cart.Quantity),
                            new SqlParameter("@itemId", cart.ItemId));

                        if (affected == 0) throw new Exception($"Item {cart.Item.Name} is out of stock.");
                    }

                    var orderHeader = new UserOrderHeader
                    {
                        TemporaryKey = dto.TemporaryKey,
                        UserId = userId,
                        DateOfOrder = DateTime.UtcNow,
                        FinalTotalAmount = pricingResult.FinalTotal,
                        PhoneNumber = dto.PhoneNumber,
                        DeliveryStreetAddress = dto.DeliveryStreetAddress,
                        City = dto.City,
                        State = dto.State,
                        PostalCode = dto.PostalCode,
                        Name = dto.Name,
                        OriginalTotalAmount = pricingResult.OriginalTotal,
                        TierDiscountAmount = pricingResult.TierDiscount,
                        TotalDiscountAmount = pricingResult.TotalDiscount,
                        PaymentType = dto.IsPayOnDelivery ? PaymentType.Cash : PaymentType.Visa,
                        OrderStatus = OrderStatus.Pending,
                        PaymentStatus = "Pending",
                        OrderItems = userCart.Select(c => new OrderItem
                        {
                            ItemId = c.ItemId,
                            Quantity = c.Quantity,
                            UnitPrice = (pricingResult.FreeProductId == c.ItemId) ? 0 : c.Item.Price,
                            IsFreeItem = (pricingResult.FreeProductId == c.ItemId)
                        }).ToList()
                    };

                    _db.UserOrderHeaders.Add(orderHeader);
                    await _db.SaveChangesAsync();

                    string? stripeUrl = null;

                    if (!dto.IsPayOnDelivery)
                    {
                        var options = new SessionCreateOptions
                        {
                            SuccessUrl = "https://lunamarofrontend.z1.web.core.windows.net/payment-success/{CHECKOUT_SESSION_ID}",
                            CancelUrl = "https://lunamarofrontend.z1.web.core.windows.net/payment-failed",
                            LineItems = new List<SessionLineItemOptions>(),
                            Mode = "payment",
                            Metadata = new Dictionary<string, string> { { "orderId", orderHeader.Id.ToString() } }
                        };

                        foreach (var item in orderHeader.OrderItems)
                        {
                            options.LineItems.Add(new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    UnitAmount = (long)(item.UnitPrice * 100),
                                    Currency = "usd",
                                    ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Food Item" },
                                },
                                Quantity = item.Quantity
                            });
                        }

                        var service = new SessionService();
                        var session = await service.CreateAsync(options);
                        stripeUrl = session.Url;
                        orderHeader.StripeSessionId = session.Id;
                        await _db.SaveChangesAsync();
                    }

                    _db.UserCarts.RemoveRange(userCart);
                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    try
                    {
                        if (OnOrderPlaced != null)
                            await OnOrderPlaced.Invoke(orderHeader);
                    }
                    catch (Exception emailEx)
                    {
                        __Loger.LogError("Email failed: {Message}", emailEx.Message);
                    }

                    return new OrderResDTO { OrderId = orderHeader.Id, PaymentUrl = stripeUrl };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    __Loger.LogError("[{ReqId}] ORDER FAILED: {Message}", requestId, ex.Message);
                    throw;
                }
            });
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
                TotalAmount = order.FinalTotalAmount,
                orderItems = order.OrderItems.Select(i => new OrderitemshistoryDTO
                {
                    ProductName = i.Item.Name,
                    ImgUrl = i.Item.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                }).ToList()
            };
        }

        public async  Task<bool> OrderSuccess(string sessionId)
        {
            var order = await _db.UserOrderHeaders
                .FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);

            if (order == null) return false;

            if (order.PaymentStatus != "Paid")
            {
                order.PaymentStatus = "Paid";
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentProcessDate = DateTime.UtcNow;

                var userCart = await _db.UserCarts.Where(u => u.UserId == order.UserId).ToListAsync();
                _db.UserCarts.RemoveRange(userCart);

                await _db.SaveChangesAsync();
            }

            return true;
        }        

        public async  Task<bool> UpdateStatusAsync(UpdateStatusOrderDTO dto, int orderId)
        {
            var order = await _db.UserOrderHeaders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = dto.Status;

            if (order.PaymentType == PaymentType.Cash && dto.Status == OrderStatus.Delivered)
            {
                order.PaymentStatus = "Paid";
            }
            await _db.SaveChangesAsync();
            if (order.OrderStatus == OrderStatus.OutForDelivery)
            {
                try
                {
                    await _notificationService.SendOutForDeliveryAsync(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to send OutForDelivery email: {ex.Message}");
                }

                if (OnOrderStatusChanged != null)
                    await OnOrderStatusChanged.Invoke(order);
            }
        
            return true;
        
        }
        public async  Task<IEnumerable<UserOrdersHistory>> UserOrderHistory()
        {
            var userId = GetCurrentUserId();



            return await _db.OrderItems.Include(x => x.UserOrderHeader).Where(x => x.UserOrderHeader.UserId == userId).Select(s => new UserOrdersHistory
            {
                OrderId = s.UserOrderHeaderId,
                DateOfOrder = s.UserOrderHeader.DateOfOrder,
                OrderStatus = s.UserOrderHeader.OrderStatus,
                TotalAmount = s.UserOrderHeader.FinalTotalAmount

            }).ToListAsync();
        }

        public async  Task<orderhistorydetails> UserOrderHistoryDetails(int orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _db.UserOrderHeaders
         .Where(x => x.UserId == userId && x.Id == orderId)
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
                TotalAmount = order.FinalTotalAmount,
                orderItems = order.OrderItems.Select(i => new OrderitemshistoryDTO
                {
                    ProductName = i.Item.Name,
                    ImgUrl = i.Item.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList()
            };
        }
    }
}
