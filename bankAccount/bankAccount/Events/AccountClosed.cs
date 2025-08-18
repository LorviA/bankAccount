namespace bankAccount.Events
{
    public record AccountClosed(
        Guid EventId,
        DateTime OccurredAt,
        Guid AccountId,
        DateTime CloseDate
    );
}
