namespace bankAccount.Events
{
    public record AccountUpdated(
        Guid EventId,
        DateTime OccurredAt,
        Guid AccountId,
        string Field,
        object OldValue,
        object NewValue
    );
}
