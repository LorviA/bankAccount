namespace bankAccount.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string? EventType { get; set; }
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string? Payload { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int RetryCount { get; set; }
    }
}
