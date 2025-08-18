using bankAccount.Data;
using bankAccount.Events;
using bankAccount.Models;
using bankAccount.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace bankAccount.Services
{
    public class OutboxProcessor(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor> logger,
        IOptions<RabbitMQOptions> options) : BackgroundService
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly ILogger<OutboxProcessor> _logger = logger;
        private readonly RabbitMQOptions _options = options.Value;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
        private readonly int _batchSize = 50;
        private readonly int _maxRetries = 5;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var factory = new ConnectionFactory
                    {
                        HostName = _options.Host,
                        Port = _options.Port,
                        UserName = _options.Username,
                        Password = _options.Password,
                        VirtualHost = _options.VirtualHost
                    };

                    var messages = await context.OutboxMessages
                        .Where(m => m.ProcessedAt == null && m.RetryCount < _maxRetries)
                        .OrderBy(m => m.CreatedAt)
                        .Take(_batchSize)
                        .ToListAsync(stoppingToken);

                    if (messages.Any())
                    {
                        // ReSharper disable once UseAwaitUsing
                        // ReSharper disable once MethodSupportsCancellation
                        using var connection = await factory.CreateConnectionAsync();
                        // ReSharper disable once UseAwaitUsing
                        // ReSharper disable once MethodSupportsCancellation
                        using var channel = await connection.CreateChannelAsync();

                        //DeclareExchanges(channel);

                        foreach (var message in messages)
                        {
                            try
                            {
                                await PublishMessageAsync(channel, message, stoppingToken);
                                message.ProcessedAt = DateTime.UtcNow;

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);
                                message.RetryCount++;
                            }

                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox messages");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task PublishMessageAsync(
            IChannel channel,
            OutboxMessage message,
            CancellationToken cancellationToken)
        {
            if (message.EventType != null)
            {
                var routingKey = GetRoutingKey(message.EventType);


                if (message.Payload != null)
                {
                    var body = Encoding.UTF8.GetBytes(message.Payload);

                    await channel.BasicPublishAsync(exchange: "account.events", routingKey: routingKey, body: body, cancellationToken: cancellationToken);
                }

                _logger.LogInformation(
                    "Published event {EventId} to {Exchange}/{RoutingKey}",
                    message.Id, _options.Exchange, routingKey);
            }
        }

        private string GetRoutingKey(string eventType) => eventType switch
        {
            nameof(AccountOpened) => "account.opened",
            nameof(AccountUpdated) => "account.updated",
            nameof(AccountDeleted) => "account.deleted",
            nameof(AccountClosed) => "account.closed",
            nameof(MoneyCredited) => "money.credited",
            nameof(MoneyDebited) => "money.debited",
            nameof(TransferCompleted) => "transfer.completed",
            //nameof(InterestAccrued) => "interest.accrued",
            _ => "#"
        };
    }
}
