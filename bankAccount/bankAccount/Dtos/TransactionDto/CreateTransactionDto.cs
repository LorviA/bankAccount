namespace bankAccount.Dtos.TransactionDto
{
    public class CreateTransactionDto
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Type { get; set; }

        public string? Description { get; set; }

        public DateTime Time { get; set; }
    }
}
