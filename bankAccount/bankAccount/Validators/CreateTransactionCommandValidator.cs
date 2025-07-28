using bankAccount.Commands;
using FluentValidation;

namespace bankAccount.Validators
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(p => p.Transaction.AccountId)
                .NotEmpty()
                .WithMessage("AccountId не может быть пустым");
            RuleFor(p => p.Transaction.Amount)
                .GreaterThan(1)
                .WithMessage("Amount должен быть больше 1");
            RuleFor(p => p.Transaction.Type)
                .Must(type => new[] { "Deposit", "Credit" }.Contains(type))
                .WithMessage("Type допустимые значения  Deposit | Credit");
            RuleFor(p => p.Transaction.Description)
                .MaximumLength(300)
                .WithMessage("Description длина не болше 300 символов");
        }
    }
}
