using bankAccount.Commands;
using FluentValidation;

namespace bankAccount.Validators
{
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once UnusedMember.Global
    public class AddAccountComandValidator: AbstractValidator<AddAccountCommand>
    {
        // ReSharper disable once IdentifierTypo
        public AddAccountComandValidator() 
        {
            RuleFor(p => p.Accont.OwnerId)
                .NotEmpty()
                .WithMessage("OwnerId не может быть пустым");
            RuleFor(p => p.Accont.Id)
                .NotEmpty()
                .WithMessage("Id не может быть пустым");
            RuleFor(p => p.Accont.InterestRate)
                .GreaterThan(0)
                .WithMessage("InterestRate должен быть больше нуля");
        }
    }
}
