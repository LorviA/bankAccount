using bankAccount.Models;

namespace bankAccount.Dtos.AccountDto
{
    public class CreateAccountDto
    {
        public Guid OwnerId { get; set; }

        public string? Type { get; set; }

        public decimal? Balance { get; set; }

        public decimal? InterestRate { get; set; }

        public DateTime OpeningDate { get; set; }

        public List<Transaction> Transactions { get; set; } = [];
    }
}
