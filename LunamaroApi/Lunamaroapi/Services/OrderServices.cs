using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
using Lunamaroapi.Helper;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Lunamaroapi.Services
{
    public delegate Task OrderPlacceHandler(UserOrderHeader order);
    public delegate Task OrderStatusChangedHandler(UserOrderHeader order);

    public class OrderServices : IOrder
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _smsService;

        public event OrderPlacceHandler? OnOrderPlaced;

        public event OrderStatusChangedHandler? OnOrderStatusChanged;



        public OrderServices(AppDBContext db, IHttpContextAccessor httpContextAccessor, EmailService emailService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _smsService = emailService;
            OnOrderPlaced += SendOrderPlacedEmail;

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

            var currentUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
                PaymentStatus = dto.IsPayOnDelivery ? "Pending Payment" : "Not Paid",
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

            // Save the order first
            await _db.UserOrderHeaders.AddAsync(orderHeader);
            await _db.SaveChangesAsync();

            // ✅ Trigger order placed email using delegate/event
            if (OnOrderPlaced != null)
            {
                await OnOrderPlaced.Invoke(orderHeader);
            }

            // If pay on delivery, clear cart and return
            if (dto.IsPayOnDelivery)
            {
                _db.UserCarts.RemoveRange(userCart);
                await _db.SaveChangesAsync();

                return new OrderResDTO
                {
                    OrderId = orderHeader.Id,
                    PaymentUrl = null // No Stripe URL
                };
            }

            // Stripe payment session
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

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(options);

            // Save Stripe info
            orderHeader.StripeSessionId = session.Id;
            orderHeader.StripePaymentIntentId = session.PaymentIntentId;
            orderHeader.PaymentStatus = "Paid";
            _db.UserOrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();

            // Clear user cart
            _db.UserCarts.RemoveRange(userCart);
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
            if (order.OrderStatus == OrderStatus.OutForDelivery)
            {
                try
                {
                    await SendOutForDeliveryEmailAsync(order);
                    Console.WriteLine($"✅ Email sent: Order #{order.Id} is out for delivery!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to send OutForDelivery email: {ex.Message}");
                }

                if (OnOrderStatusChanged != null)
                    await OnOrderStatusChanged.Invoke(order);
            }


            // This will now always run
            await _db.SaveChangesAsync();


            // Save changes to database

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
        private async Task SendOrderPlacedEmail(UserOrderHeader order)
        {
            if (string.IsNullOrEmpty(order.UserId)) return;

            // Get user email (from DB or order object)
            var userEmail = order.User?.Email ?? "customer@example.com";
            var userName = order.Name ?? "Customer";

            string subject = $"✅ Your Order #{order.Id} Has Been Placed Successfully!";

            string body = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
            .container {{ max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
            .header {{ background-color: #4CAF50; color: white; padding: 10px; text-align: center; border-radius: 8px 8px 0 0; }}
            .content {{ padding: 20px; }}
            .button {{ display: inline-block; padding: 10px 20px; margin-top: 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; }}
            .footer {{ margin-top: 20px; font-size: 12px; color: #777; text-align: center; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h2>Order Confirmation</h2>
            </div>
            <div class='content'>
                <p>Hi {userName},</p>
                <p>Thank you for your order! Your order #{order.Id} has been placed successfully and is being processed.</p>
                <p>Order Date: {order.DateOfOrder:dd MMM yyyy}</p>
                <a class='button' href='#'>View Your Order</a>
            </div>
            <div class='footer'>
                <p>Thank you for shopping with us!<br/>YourCompanyName</p>
            </div>
        </div>
    </body>
    </html>";

            await _smsService.SendEmailAsync(userEmail, subject, body);
        }
        private async Task SendOutForDeliveryEmailAsync(UserOrderHeader order)
        {
            if (string.IsNullOrEmpty(order?.UserId))
                return;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return;

            string subject = $"🚚 Your Order #{order.Id} is Out for Delivery!";
            string body = $@"
        <h3>Hi {user.FullName ?? "Customer"},</h3>
        <p>Good news! Your order <strong>#{order.Id}</strong> is now <strong>out for delivery</strong>.</p>
        <p>It should arrive at your address within approximately <strong>30 minutes</strong>.</p>
        <p>Thank you for shopping with <strong>Lunamaro</strong>! ❤️</p>
        <hr />
        <p><small>If you have any questions, contact our support team anytime.</small></p>";

            await _smsService.SendEmailAsync(user.Email, subject, body);
        }



    }
}
