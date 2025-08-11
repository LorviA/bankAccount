namespace bankAccount.Models
{
    public enum TransactionType { Credit, Debit }
    public class Transaction
    {
        public Transaction()
        {}

        public Transaction(int type, decimal amount, string? currency, string? description)
        {
            Id = Guid.NewGuid();
            AccountId = Guid.NewGuid();
            CounterpartyAccountId = Guid.NewGuid();
            Amount = amount;
            Currency = currency;
            Description = description;
            Type = (TransactionType)type;
            Time = DateTime.UtcNow;
        }

        public Transaction(Guid accountId, Guid counterpartyAccountId, int type, decimal amount, string? currency, string? description)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            CounterpartyAccountId = counterpartyAccountId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Type = (TransactionType)type;
            Time = DateTime.UtcNow;
        }
        /// <summary>
        /// Unique identifier for the transaction
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of the account owner (user)
        /// </summary>
        /// <example>a8a110d5-fc49-43c5-bf46-802a8e92d966</example>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The identifier of the counterparty's (user's) account holder
        /// </summary>
        /// <example>a8a110d5-fc49-43c5-bf46-802a8e92d966</example>
        public Guid CounterpartyAccountId { get; set; }

        /// <summary>
        ///The transaction amount must be greater than 0
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code according to ISO 4217 standard
        /// </summary>
        /// <example>USD</example>
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string? Currency { get; set; }// ISO 4217

        /// <summary>
        /// 0 - Credit (coming), 1 - Debit (expenditure)
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        ///  max length 300
        /// </summary>
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string? Description { get; set; }

        public DateTime Time { get; set; }
    }
}
