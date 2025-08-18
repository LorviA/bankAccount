using bankAccount.Data;
using bankAccount.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace bankAccount.Services
{
    public class AntifraudConsumer : BackgroundService
    {
        private readonly IChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AntifraudConsumer> _logger;
        private const string QueueName = "account.antifraud";

        public AntifraudConsumer(
            IConnection connection,
            IServiceProvider serviceProvider,
            ILogger<AntifraudConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            ConfigureRabbitMq();
        }

        private void ConfigureRabbitMq()
        {
            _channel.ExchangeDeclareAsync("account.events", ExchangeType.Topic, durable: true);
            _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBindAsync(QueueName, "account.events", "client.*");
            _channel.BasicQosAsync(0, 1, false); // Обработка по одному сообщению
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    switch (routingKey)
                    {
                        case "client.blocked":
                            var blockedEvent = JsonSerializer.Deserialize<ClientBlocked>(message);
                            if (blockedEvent != null) await HandleClientBlocked(blockedEvent);
                            break;
                        case "client.unblocked":
                            var unblockedEvent = JsonSerializer.Deserialize<ClientUnblocked>(message);
                            if (unblockedEvent != null) await HandleClientUnblocked(unblockedEvent);
                            break;
                    }

                    // ReSharper disable once MethodSupportsCancellation
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _channel.BasicAckAsync(ea.DeliveryTag, false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing antifraud message");
                    // ReSharper disable once MethodSupportsCancellation
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _channel.BasicNackAsync(ea.DeliveryTag, false, true);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            };

            // ReSharper disable once MethodSupportsCancellation
            _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleClientBlocked(ClientBlocked @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var accounts = await context.Accounts
                .Where(a => a.OwnerId == @event.ClientId)
                .ToListAsync();

            foreach (var account in accounts)
            {
                account.CloseDate = DateTime.UtcNow;
                _logger.LogInformation("Account {AccountId} frozen for client {ClientId}",
                    account.Id, @event.ClientId);
            }

            await context.SaveChangesAsync();
        }

        private async Task HandleClientUnblocked(ClientUnblocked @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var accounts = await context.Accounts
                .Where(a => a.OwnerId == @event.ClientId)
                .ToListAsync();

            foreach (var account in accounts)
            {
                account.CloseDate = null;
                _logger.LogInformation("Account {AccountId} unfrozen for client {ClientId}",
                    account.Id, @event.ClientId);
            }

            await context.SaveChangesAsync();
        }

        public override void Dispose()
        {
            _channel.CloseAsync();
            base.Dispose();
        }
    }
}