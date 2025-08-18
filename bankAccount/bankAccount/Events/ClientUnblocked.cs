namespace bankAccount.Events
{
    public record ClientUnblocked(
        Guid EventId,
        DateTime OccurredAt,
        Guid ClientId
    );
}
