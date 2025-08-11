using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace bankAccount.Models
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum AccountType { Checking, Deposit, Credit }

    public class Account
    {
        public Account()
        {}

        public Account(int type, string? currency, decimal? balance, decimal? interestRate,
            DateTime openingDate)
        {
            Id = Guid.NewGuid();
            OwnerId = Guid.NewGuid();
            Type = (AccountType)type;
            Currency = currency;
            Balance = balance;
            InterestRate = interestRate;
            OpeningDate = openingDate;
            Transactions = [];
        }
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public AccountType Type { get; set; }

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string? Currency { get; set; } // ISO 4217

        public decimal? Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpeningDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<Transaction> Transactions { get; set; } = [];

        [ConcurrencyCheck]
        public byte[]? Version { get; set; }
    }
}
