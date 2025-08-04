using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using FluentValidation;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, MbResult<Account>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<Account> _validator;

        public AddAccountCommandHandler(
            IAccountRepository accountRepository,
            IValidator<Account> validator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
        }

        public async Task<MbResult<Account>> Handle(
            AddAccountCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Accont);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                    );

                return MbResult<Account>.Failure(
                    MbError.ValidationError(errors)
                );
            }

            var createdAccount = await _accountRepository.AddProduct(request.Accont);
            return MbResult<Account>.Success(createdAccount);
        }
    }
}
