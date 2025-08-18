using System.Diagnostics;
using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class CloseAccountHandler(IAccountRepository accountRepository) : IRequestHandler<CloseAccountCommand, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<Account>> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование аккаунта
                var existingAccount = await _accountRepository.GetProductById(request.Id);
                if (existingAccount == null)
                {
                    return MbResult<Account>.Failure(
                        MbError.NotFound($"Account with ID {request.Id} not found")
                    );
                }
                var updatedAccount = await _accountRepository.CloseAccountById(request.Id);
                Debug.Assert(updatedAccount != null, nameof(updatedAccount) + " != null");
                return MbResult<Account>.Success(updatedAccount);
            }
            catch (Exception ex)
            {
                return MbResult<Account>.Failure(
                    MbError.Internal($"Error updating account: {ex.Message}")
                );
            }
        }
    }
}
