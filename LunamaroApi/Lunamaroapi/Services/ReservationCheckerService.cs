using Lunamaroapi.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services
{
    public class ReservationCheckerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCheckerService> _logger;

        public ReservationCheckerService(IServiceProvider serviceProvider, ILogger<ReservationCheckerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckReservationsAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckReservationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var now = DateTime.UtcNow;

            _logger.LogInformation($"⏰ Checking for expired reservations at {now}");

            var expiredReservations = await context.Reservations
    .Where(r => r.ReservationEnd <= now && r.Status != null && r.Status.ToLower() == "reserved")
    .ToListAsync();

            foreach (var res in expiredReservations)
            {
                _logger.LogInformation($"🔎 Expired Reservation Found: Id={res.Id}, TableId={res.TableId}, EndTime={res.ReservationEnd}");
                res.Status = "available";
            }

            if (expiredReservations.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation($"✅ {expiredReservations.Count} reservations marked as Available at {now}.");
            }
            else
            {
                _logger.LogInformation($"ℹ️ No expired reservations found at {now}.");
            }
        }

    }
}
