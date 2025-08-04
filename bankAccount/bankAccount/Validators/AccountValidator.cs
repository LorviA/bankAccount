using FluentValidation;
using bankAccount.Models;

namespace bankAccount.Validators
{
    public class AccountValidator : AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(a => a.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required");

            RuleFor(a => a.Type)
                .IsInEnum().WithMessage("Invalid account type");

            RuleFor(a => a.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Balance cannot be negative");

            RuleFor(a => a.InterestRate)
                .GreaterThan(0).When(a => a.InterestRate.HasValue)
                .WithMessage("Interest rate must be positive");

            RuleFor(a => a.OpeningDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Opening date cannot be in the future");
        }
    }
}