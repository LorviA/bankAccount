using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class UpdateAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<UpdateAccountCommand, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<Account>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
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
                var updatedAccount = await _accountRepository.UpdateProdict(request.Id, request.Accont);
                if (updatedAccount == null)
                {
                    return MbResult<Account>.Failure(
                        MbError.NotFound($"Account with ID {request.Id} not found")
                    );
                }
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
