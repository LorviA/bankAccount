namespace bankAccount.Models
{
    public enum AccountType { Checking, Deposit, Credit }

    public class Account
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public AccountType Type { get; set; }

        public string? Currency { get; set; } // ISO 4217

        public decimal? Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpeningDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<Transaction> Transactions { get; set; } = [];
    }
}
