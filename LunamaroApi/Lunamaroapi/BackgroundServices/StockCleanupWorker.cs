

using Lunamaroapi.Data;
using Lunamaroapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.BackgroundServices
{
    public class StockCleanupWorker : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockCleanupWorker> _logger;

        public StockCleanupWorker(IServiceProvider serviceProvider, ILogger<StockCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Stock Cleanup Worker running at: {time}", DateTimeOffset.Now);
                using (var scoped = _serviceProvider.CreateScope())
                {
                    var db = scoped.ServiceProvider.GetRequiredService<AppDBContext>();
                    var thresholdTime = DateTime.Now.AddMinutes(-10);
                    var abandonedOrders = await db.UserOrderHeaders.Include(c => c.OrderItems).Where(o => o.PaymentType == PaymentType.Visa
                    && o.PaymentStatus == "Pending" && o.DateOfOrder < thresholdTime).ToListAsync();

                    if (abandonedOrders.Any())
                    {
                        foreach (var order in abandonedOrders)
                        {
                            _logger.LogWarning($"Restoring stock for Abandoned Order #{order.Id}");
                            foreach (var item in order.OrderItems)
                            {
                                var product = await db.Items.FindAsync(item.ItemId);
                                if (product != null)
                                {
                                    product.quantity += item.Quantity;
                                }
                            }
                            order.PaymentStatus = "Timed Out";
                            order.OrderStatus = OrderStatus.Cancelled;

                        }
                        await db.SaveChangesAsync();
                        _logger.LogInformation($"Successfully cleaned up {abandonedOrders.Count} orders.");
                    }



                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
