namespace bankAccount.Events
{
    public record AccountDeleted(
        Guid EventId,
        DateTime OccurredAt,
        Guid AccountId,
        DateTime DeletionDate
    );
}
