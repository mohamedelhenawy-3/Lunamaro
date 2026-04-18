using Lunamaroapi.Data;
using Lunamaroapi.Queues;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services.Implements
{
    public class OrderNotificationService : IOrderNotificationService
    {

        private readonly AppDBContext _db;

        public OrderNotificationService(AppDBContext db)
        {
            _db = db;
        }

        public async Task SendOrderPlacedAsync(UserOrderHeader order)
        {
            if (string.IsNullOrEmpty(order.UserId)) return;

            var userEmail = order.User?.Email ?? "customer@example.com";
            var userName = order.Name ?? "Customer";

            string subject = $"✅ Your Order #{order.Id} Has Been Placed Successfully!";

            string body = BuildOrderPlacedTemplate(userName, order);

            EmailQueue.Queue.Enqueue((userEmail, subject, body));
        }

        public async  Task SendOutForDeliveryAsync(UserOrderHeader order)
        {
            if (string.IsNullOrEmpty(order?.UserId))
                return;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return;

            string subject = $"🚚 Your Order #{order.Id} is Out for Delivery!";

            string body = BuildOutForDeliveryTemplate(user.FullName, order.Id);

            EmailQueue.Queue.Enqueue((user.Email, subject, body));
        }
        private string BuildOrderPlacedTemplate(string userName, UserOrderHeader order)
        {
            return $@"
<html>
<head>
    <style>
        body {{ font-family: Arial; }}
        .container {{ max-width:600px; margin:auto; padding:20px; border:1px solid #ddd; border-radius:8px; }}
        .header {{ background:#4CAF50; color:white; padding:10px; text-align:center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Order Confirmation</h2>
        </div>
        <p>Hi {userName},</p>
        <p>Your order #{order.Id} has been placed successfully.</p>
        <p>Date: {order.DateOfOrder:dd MMM yyyy}</p>
    </div>
</body>
</html>";
        }

        private string BuildOutForDeliveryTemplate(string name, int orderId)
        {
            return $@"
<h3>Hi {name ?? "Customer"},</h3>
<p>Your order <strong>#{orderId}</strong> is out for delivery 🚚</p>
<p>Arriving soon!</p>";
        }
    }
}
