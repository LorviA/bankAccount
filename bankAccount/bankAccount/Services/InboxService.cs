using bankAccount.Data;
using bankAccount.Models;
using Microsoft.EntityFrameworkCore;

namespace bankAccount.Services
{
    // ReSharper disable once UnusedMember.Global
    public class InboxService
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private readonly ApplicationDbContext _context;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        // ReSharper disable once UnusedMember.Global
        public async Task<bool> HasBeenProcessed(Guid messageId)
        {
            return await _context.InboxConsumed
                .AnyAsync(c => c.MessageId == messageId);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task MarkAsProcessed(Guid messageId, string handlerName)
        {
            _context.InboxConsumed.Add(new InboxConsumed
            {
                MessageId = messageId,
                ProcessedAt = DateTime.UtcNow,
                Handler = handlerName
            });

            await _context.SaveChangesAsync();
        }
    }
}
