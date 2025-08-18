using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using System.Text.Json;

namespace bankAccount.Services
{
    public class OutboxService(ApplicationDbContext context, ILogger<OutboxService> logger) : IOutboxService
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly ApplicationDbContext _context = context;
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly ILogger<OutboxService> _logger = logger;

        public async Task AddEventAsync(object @event, CancellationToken cancellationToken)
        {
            var eventType = @event.GetType().Name;
            var payload = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                CreatedAt = DateTime.UtcNow
            };

            // Добавляем сообщение в ту же транзакцию
            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Event added to outbox: {EventId}", outboxMessage.Id);

        }
    }
}
