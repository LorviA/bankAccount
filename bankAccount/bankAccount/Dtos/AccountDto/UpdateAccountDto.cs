using bankAccount.Models;

namespace bankAccount.Dtos.AccountDto
{
    public class UpdateAccountDto
    {
        /// <summary>
        /// Identifier of the account owner (user)
        /// </summary>
        /// <example>a8a110d5-fc49-43c5-bf46-802a8e92d966</example>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 0 - Checking, 1 - Deposit, 2 - Credit 
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        /// Current account balance.
        /// </summary>
        /// <example>1500.75</example>
        public decimal? Balance { get; set; }

        /// <summary>
        /// Currency code according to ISO 4217 standard
        /// </summary>
        /// <example>USD</example>
        public string? Currency { get; set; } // ISO 4217

        public decimal? InterestRate { get; set; }

        /// <summary>
        /// Date when the account was closed (null if active)
        /// </summary>
        /// <example>2025-12-31T23:59:59Z</example>
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// List of transactions associated with this account
        /// </summary>
        public List<Transaction> Transactions { get; set; } = [];
    }
}
