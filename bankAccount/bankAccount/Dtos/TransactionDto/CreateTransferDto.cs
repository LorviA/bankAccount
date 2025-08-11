using bankAccount.Models;
// ReSharper disable once IdentifierTypo
namespace bankAccount.Dtos.TransactionDto
{
    public class CreateTransferDto
    {
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
        public string? Currency { get; set; }// ISO 4217

        /// <summary>
        /// 0 - Credit (coming), 1 - Debit (expenditure)
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        ///  max length 300
        /// </summary>
        public string? Description { get; set; }

        public DateTime Time { get; set; }
    }
}
