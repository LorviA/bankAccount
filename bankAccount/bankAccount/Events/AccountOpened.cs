using bankAccount.Models;

namespace bankAccount.Events
{
    public record AccountOpened(
        Guid EventId,
        DateTime OccurredAt,
        Guid AccountId,
        Guid OwnerId,
        string Currency,
        AccountType Type
    );
}
