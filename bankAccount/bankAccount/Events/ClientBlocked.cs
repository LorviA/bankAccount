namespace bankAccount.Events
{
    public record ClientBlocked
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid ClientId
    );
}
