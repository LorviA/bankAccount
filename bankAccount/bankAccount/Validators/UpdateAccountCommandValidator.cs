using bankAccount.Commands;
using FluentValidation;

namespace bankAccount.Validators
{
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(p => p.Accont.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId не может быть пустым");
            RuleFor(p => p.Accont.Id)
                .NotEmpty()
                .WithMessage("Id не может быть пустым");
            RuleFor(p => p.Accont.Type)
                .Must(type => new[] { "Checking", "Deposit", "Credit" }.Contains(type))
                .WithMessage("Type допустимые значения Checking | Deposit | Credit");
            RuleFor(p => p.Accont.InterestRate)
                .GreaterThan(0)
                .WithMessage("InterestRate должен быть больше нуля");
        }
    }
}
