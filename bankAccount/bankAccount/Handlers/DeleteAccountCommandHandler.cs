using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class DeleteAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteAccountCommand, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<Account>> Handle(
            DeleteAccountCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var existingAccount = await _accountRepository.GetProductById(request.Id);
                if (existingAccount == null)
                {
                    return MbResult<Account>.Failure(
                        MbError.NotFound($"Account with ID {request.Id} not found")
                    );
                }

                var deletedAccount = await _accountRepository.DeleteProduct(request.Id);

                if (deletedAccount == null)
                {
                    return MbResult<Account>.Failure(
                        MbError.Internal($"Failed to delete account with ID {request.Id}")
                    );
                }

                return MbResult<Account>.Success(deletedAccount);
            }
            catch (Exception ex)
            {
                return MbResult<Account>.Failure(
                    MbError.Internal($"Error deleting account: {ex.Message}")
                );
            }
        }
    }
}