using Lunamaroapi.Queues;

namespace Lunamaroapi.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (EmailQueue.Queue.TryDequeue(out var emailData))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                        await emailService.SendEmailAsync(emailData.Email, emailData.Subject, emailData.Body);

                        // Small delay between emails to avoid spamming the provider
                        await Task.Delay(500, stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(5000, stoppingToken);
                    }
                }
            }
        }
    }
}
