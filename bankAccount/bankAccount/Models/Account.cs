namespace bankAccount.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string? Type { get; set; }

        public decimal? Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpeningDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<Transaction> Transactions { get; set; } = [];
    }
}
