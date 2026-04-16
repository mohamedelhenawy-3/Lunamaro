
using Lunamaroapi.Queues;

namespace Lunamaroapi.BackgroundServices
{
    public class OrderEmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderEmailBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                if (EmailQueue.Queue.TryDequeue(out var emailData))
                {
                    using var scope = _serviceProvider.CreateScope();

                    var emailService = scope.ServiceProvider
                        .GetRequiredService<EmailService>();


                    emailService.SendEmailAsync(emailData.Email, emailData.Subject, emailData.Body);

                    await Task.Delay(500, stoppingToken);
                }
            }
        }
    }
}
