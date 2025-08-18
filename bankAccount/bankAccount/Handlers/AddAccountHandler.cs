using System.Diagnostics;
using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using FluentValidation;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddAccountCommandHandler(
        IAccountRepository accountRepository,
        IValidator<Account> validator) : IRequestHandler<AddAccountCommand, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IValidator<Account> _validator = validator;

        public async Task<MbResult<Account>> Handle(
            AddAccountCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Accont, cancellationToken);

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
            Debug.Assert(createdAccount != null, nameof(createdAccount) + " != null");
            return MbResult<Account>.Success(createdAccount);
        }
    }
}
