namespace bankAccount.Events
{
    public record MoneyDebited
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid AccountId,
        decimal? Amount,
        string? Currency,
        Guid OperationId,
        string? Reason
    );
}
