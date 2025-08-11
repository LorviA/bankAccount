using bankAccount.Commands;
using FluentValidation;

namespace bankAccount.Validators
{
    // ReSharper disable once UnusedMember.Global
    public class AddTransferCommandValidator: AbstractValidator<AddTransferCommand>
    {
        public AddTransferCommandValidator()
        {
            RuleFor(p => p.Transfer.AccountId)
                .NotEmpty()
                .WithMessage("AccountId не может быть пустым");
            RuleFor(p => p.Transfer.CounterpartyAccountId)
                .NotEmpty()
                .WithMessage("CounterpartyAccountId не может быть пустым");
            RuleFor(p => p.Transfer.Amount)
                .GreaterThan(1)
                .WithMessage("Amount должен быть больше 1");
            RuleFor(p => p.Transfer.Description)
                .MaximumLength(300)
                .WithMessage("Description длина не болше 300 символов");
        }
    }
}
