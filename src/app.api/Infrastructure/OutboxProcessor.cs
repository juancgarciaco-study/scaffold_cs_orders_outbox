using Microsoft.EntityFrameworkCore;

namespace app.api.Infrastructure;

public class OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            logger.LogInformation("Processing outbox messages ...");

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOnUtc == null)
                    .OrderBy(m => m.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        // Here you would typically publish the message to a message bus (e.g., RabbitMQ, Azure Service Bus)
                        logger.LogInformation("Publishing message: {Type} - {Content}", message.MessageType, message.Content);

                        message.ProcessedOnUtc = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        message.Error = ex.ToString();
                        logger.LogError(ex, "Error processing message {MessageId}", message.Id);
                    }
                }

                if (messages.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OutboxProcessor");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
