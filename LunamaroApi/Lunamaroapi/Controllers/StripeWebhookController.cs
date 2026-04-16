using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Lunamaroapi.Models; // Ensure this points to where your OrderStatus enum is

[Route("api/[controller]")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly string _webhookSecret;

    public StripeWebhookController(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        // This reads from your appsettings.json
        _webhookSecret = config["StripeSettings:WebhookSecret"];
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _webhookSecret);

            // Using the string directly to avoid SDK version conflicts
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                // Find the order using the StripeSessionId you have in your model
                var orderHeader = await _db.UserOrderHeaders
                    .FirstOrDefaultAsync(u => u.StripeSessionId == session.Id);

                if (orderHeader != null)
                {
                    // Update the status using your Model properties
                    orderHeader.PaymentStatus = "Paid";
                    orderHeader.OrderStatus = OrderStatus.Pending; // Using your Enum
                    orderHeader.PaymentProcessDate = DateTime.Now;
                    orderHeader.TransactionId = session.PaymentIntentId;

                    await _db.SaveChangesAsync();

                    Console.WriteLine($"Lunamaro Success: Order {orderHeader.Id} updated to Paid.");
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook Error: {ex.Message}");
            return BadRequest();
        }
    }
}