using bankAccount.Models;

namespace bankAccount.Dtos.AccountDto
{
    public class UpdateAccountDto
    {
        public Guid OwnerId { get; set; }
        public string? Type { get; set; }

        public decimal? Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime? CloseDate { get; set; }

        public List<Transaction> Transactions { get; set; } = [];
    }
}
