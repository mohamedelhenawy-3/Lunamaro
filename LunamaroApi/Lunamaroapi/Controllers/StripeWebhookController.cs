using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Lunamaroapi.Models;
using Lunamaroapi.Data; // Ensure this points to where your OrderStatus enum is

[Route("api/[controller]")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly string _webhookSecret;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<WebhookController> logger)
    {
        _webhookSecret = configuration["StripeSettings:WebhookSecret"];
        _serviceProvider = serviceProvider;
        _logger = logger;
    }



    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _webhookSecret
            );

            // 1. Only listen for successful checkouts
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                // 2. Get the OrderId you stored in the session earlier
                var orderIdClaim = session.Metadata.ContainsKey("OrderId")
                                   ? session.Metadata["OrderId"]
                                   : session.ClientReferenceId;

                if (!string.IsNullOrEmpty(orderIdClaim))
                {
                    await ProcessOrderPayment(int.Parse(orderIdClaim), session.PaymentIntentId);
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError($"Webhook error: {e.Message}");
            return BadRequest();
        }
    }

    private async Task ProcessOrderPayment(int orderId, string paymentIntentId)  
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

        var order = await db.UserOrderHeaders.FindAsync(orderId);

        if (order.PaymentStatus == "Timed Out" || order.OrderStatus == OrderStatus.Cancelled)
        {
            _logger.LogCritical($"URGENT: Payment received for ALREADY CANCELLED order #{orderId}. Manual refund required!");
            return;
        }


        order.PaymentStatus = "Paid";
        order.OrderStatus = OrderStatus.Processing; 
        order.TransactionId = paymentIntentId;

        await db.SaveChangesAsync();
        _logger.LogInformation($"Order #{orderId} updated to Paid.");
    }


}