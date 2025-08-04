using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, MbResult<Account>>
    {
        private readonly IAccountRepository _accountRepository;

        public UpdateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

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
