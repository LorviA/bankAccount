using bankAccount.Data;
using bankAccount.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;

namespace bankAccount.Services
{
    // ReSharper disable once UnusedMember.Global
    public abstract class BaseConsumer(
        IServiceProvider serviceProvider,
        ILogger logger,
        string handlerName)
    {
        protected readonly IServiceProvider ServiceProvider = serviceProvider;
        // ReSharper disable once InconsistentNaming
        protected readonly ILogger _logger = logger;
        // ReSharper disable once InconsistentNaming
        protected readonly string _handlerName = handlerName;

        // ReSharper disable once UnusedMember.Global
        protected async Task<bool> TryHandleMessageAsync(
            BasicDeliverEventArgs ea,
            Func<Task> handleAction)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var messageId = Guid.Parse(ea.BasicProperties.MessageId);
#pragma warning restore CS8604 // Possible null reference argument.

            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Проверка на дубликат
            if (await context.InboxConsumed
                    .AnyAsync(c => c.MessageId == messageId && c.Handler == _handlerName))
            {
                _logger.LogWarning("Message {MessageId} already processed", messageId);
                return true; // ACK
            }

            try
            {
                await handleAction();

                // Сохраняем факт обработки
                context.InboxConsumed.Add(new InboxConsumed
                {
                    MessageId = messageId,
                    ProcessedAt = DateTime.UtcNow,
                    Handler = _handlerName
                });

                await context.SaveChangesAsync();
                return true; // ACK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", messageId);
                return false; // NACK
            }
        }
    }
}
