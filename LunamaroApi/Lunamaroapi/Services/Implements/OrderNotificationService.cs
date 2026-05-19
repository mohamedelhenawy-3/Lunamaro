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
            string subject = $"✅ Order #{order.Id} Confirmed – Lunamaro Restaurant";
            string body = BuildOrderPlacedTemplate(userName, order);
            EmailQueue.Queue.Enqueue((userEmail, subject, body));
        }

        public async Task SendOutForDeliveryAsync(UserOrderHeader order)
        {
            if (string.IsNullOrEmpty(order?.UserId)) return;
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null || string.IsNullOrEmpty(user.Email)) return;
            string subject = $"🚚 Your Order #{order.Id} is On Its Way!";
            string body = BuildOutForDeliveryTemplate(user.FullName, order);
            EmailQueue.Queue.Enqueue((user.Email, subject, body));
        }

        private string BuildOrderPlacedTemplate(string userName, UserOrderHeader order)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ margin:0; padding:0; background:#0f1c2e; font-family:'Segoe UI',Arial,sans-serif; }}
    .wrapper {{ max-width:600px; margin:40px auto; background:#1a2e46; border-radius:16px; overflow:hidden; border:1px solid rgba(239,176,54,0.2); }}
    .header {{ background:#1a2e46; padding:40px 40px 20px; text-align:center; border-bottom:1px solid rgba(239,176,54,0.2); }}
    .logo {{ font-size:2rem; font-weight:800; color:#EFB036; letter-spacing:-1px; }}
    .badge {{ display:inline-block; background:rgba(52,211,153,0.15); color:#34d399; border:1px solid rgba(52,211,153,0.3); padding:6px 18px; border-radius:20px; font-size:0.85rem; font-weight:600; margin-top:12px; }}
    .body {{ padding:35px 40px; }}
    .greeting {{ color:#ffffff; font-size:1.1rem; margin-bottom:20px; }}
    .detail-card {{ background:rgba(255,255,255,0.04); border:1px solid rgba(239,176,54,0.15); border-radius:12px; padding:24px; margin:20px 0; }}
    .detail-row {{ display:flex; justify-content:space-between; padding:10px 0; border-bottom:1px solid rgba(255,255,255,0.06); }}
    .detail-row:last-child {{ border-bottom:none; }}
    .detail-label {{ color:rgba(255,255,255,0.5); font-size:0.85rem; text-transform:uppercase; letter-spacing:1px; }}
    .detail-value {{ color:#EFB036; font-weight:600; font-size:0.95rem; }}
    .info-box {{ background:rgba(239,176,54,0.07); border:1px solid rgba(239,176,54,0.2); border-radius:10px; padding:16px 20px; margin:20px 0; color:rgba(255,255,255,0.75); font-size:0.9rem; line-height:1.7; }}
    .note {{ color:rgba(255,255,255,0.6); font-size:0.88rem; line-height:1.6; margin-top:20px; }}
    .footer {{ background:rgba(0,0,0,0.2); padding:20px 40px; text-align:center; color:rgba(255,255,255,0.3); font-size:0.8rem; }}
    .gold {{ color:#EFB036; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <div class='logo'>Lunamaro</div>
      <div class='badge'>✓ Order Confirmed</div>
    </div>
    <div class='body'>
      <p class='greeting'>Dear <strong class='gold'>{userName}</strong>,</p>
      <p style='color:rgba(255,255,255,0.7);line-height:1.7;'>
        Thank you for your order! We've received it and our kitchen is already preparing your meal with care.
      </p>

      <div class='detail-card'>
        <div class='detail-row'>
          <span class='detail-label'>Order ID</span>
          <span class='detail-value'>#{order.Id}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Date</span>
          <span class='detail-value'>{order.DateOfOrder:dddd, MMMM dd yyyy}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Delivery Address</span>
          <span class='detail-value'>{order.DeliveryStreetAddress}, {order.City}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Phone</span>
          <span class='detail-value'>{order.PhoneNumber}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Total Amount</span>
          <span class='detail-value'>EGP {order.FinalTotalAmount:0.00}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Payment</span>
          <span class='detail-value'>{order.PaymentType}</span>
        </div>
      </div>

      <div class='info-box'>
        🕐 <strong style='color:#EFB036;'>Estimated Delivery Time: 10 – 30 minutes</strong><br>
        Our team is preparing your order right now. Delivery time may vary slightly depending on your location and current demand.
      </div>

      <p class='note'>
        📍 <strong style='color:#fff;'>Ahmed Oraby Street, Giza, Egypt</strong><br>
        📞 +20 015 5660 59<br><br>
        If you have any questions about your order, please don't hesitate to contact us.
      </p>
    </div>
    <div class='footer'>
      © {DateTime.Now.Year} Lunamaro Restaurant. All rights reserved.<br>
      This is an automated confirmation email.
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOutForDeliveryTemplate(string userName, UserOrderHeader order)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ margin:0; padding:0; background:#0f1c2e; font-family:'Segoe UI',Arial,sans-serif; }}
    .wrapper {{ max-width:600px; margin:40px auto; background:#1a2e46; border-radius:16px; overflow:hidden; border:1px solid rgba(239,176,54,0.2); }}
    .header {{ background:#1a2e46; padding:40px 40px 20px; text-align:center; border-bottom:1px solid rgba(239,176,54,0.2); }}
    .logo {{ font-size:2rem; font-weight:800; color:#EFB036; letter-spacing:-1px; }}
    .badge {{ display:inline-block; background:rgba(59,130,246,0.15); color:#60a5fa; border:1px solid rgba(59,130,246,0.3); padding:6px 18px; border-radius:20px; font-size:0.85rem; font-weight:600; margin-top:12px; }}
    .body {{ padding:35px 40px; }}
    .greeting {{ color:#ffffff; font-size:1.1rem; margin-bottom:20px; }}
    .detail-card {{ background:rgba(255,255,255,0.04); border:1px solid rgba(239,176,54,0.15); border-radius:12px; padding:24px; margin:20px 0; }}
    .detail-row {{ display:flex; justify-content:space-between; padding:10px 0; border-bottom:1px solid rgba(255,255,255,0.06); }}
    .detail-row:last-child {{ border-bottom:none; }}
    .detail-label {{ color:rgba(255,255,255,0.5); font-size:0.85rem; text-transform:uppercase; letter-spacing:1px; }}
    .detail-value {{ color:#EFB036; font-weight:600; font-size:0.95rem; }}
    .delivery-banner {{ background:rgba(59,130,246,0.08); border:1px solid rgba(59,130,246,0.25); border-radius:10px; padding:20px; margin:20px 0; text-align:center; }}
    .delivery-icon {{ font-size:2.5rem; margin-bottom:8px; }}
    .delivery-text {{ color:#60a5fa; font-size:1rem; font-weight:600; }}
    .info-box {{ background:rgba(239,176,54,0.07); border:1px solid rgba(239,176,54,0.2); border-radius:10px; padding:16px 20px; margin:20px 0; color:rgba(255,255,255,0.75); font-size:0.9rem; line-height:1.7; }}
    .note {{ color:rgba(255,255,255,0.6); font-size:0.88rem; line-height:1.6; margin-top:20px; }}
    .footer {{ background:rgba(0,0,0,0.2); padding:20px 40px; text-align:center; color:rgba(255,255,255,0.3); font-size:0.8rem; }}
    .gold {{ color:#EFB036; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <div class='logo'>Lunamaro</div>
      <div class='badge'>🚚 Out for Delivery</div>
    </div>
    <div class='body'>
      <p class='greeting'>Dear <strong class='gold'>{userName ?? "Customer"}</strong>,</p>
      <p style='color:rgba(255,255,255,0.7);line-height:1.7;'>
        Great news! Your order is on its way and will arrive at your door shortly.
      </p>

      <div class='delivery-banner'>
        <div class='delivery-icon'>🚚</div>
        <div class='delivery-text'>Your order is heading your way!</div>
      </div>

      <div class='detail-card'>
        <div class='detail-row'>
          <span class='detail-label'>Order ID</span>
          <span class='detail-value'>#{order.Id}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Delivery Address</span>
          <span class='detail-value'>{order.DeliveryStreetAddress}, {order.City}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Total Amount</span>
          <span class='detail-value'>EGP {order.FinalTotalAmount:0.00}</span>
        </div>
      </div>

      <div class='info-box'>
        ⏱️ <strong style='color:#EFB036;'>Arriving in approximately 10 – 30 minutes</strong><br>
        Please make sure someone is available to receive the order at your delivery address.
      </div>

      <p class='note'>
        📍 <strong style='color:#fff;'>Ahmed Oraby Street, Giza, Egypt</strong><br>
        📞 +20 015 5660 59<br><br>
        If you have any issues with your delivery, please call us immediately.
      </p>
    </div>
    <div class='footer'>
      © {DateTime.Now.Year} Lunamaro Restaurant. All rights reserved.<br>
      This is an automated notification email.
    </div>
  </div>
</body>
</html>";
        }
    }
}