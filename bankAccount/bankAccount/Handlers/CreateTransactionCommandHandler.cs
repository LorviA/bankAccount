using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class CreateTransactionCommandHandler(IAccountRepository accountRepository) : IRequestHandler<CreateTransactionCommand, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<Account>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _accountRepository.CreateTransaction(request.Id,request.Transaction);

                if (result == null)
                {
                    return MbResult<Account>.Failure(
                        MbError.NotFound("Transfer failed: invalid accounts or insufficient funds")
                    );
                }

                return MbResult<Account>.Success(result);
            }
            catch (Exception ex)
            {
                return MbResult<Account>.Failure(
                    MbError.Internal($"Error processing transfer: {ex.Message}")
                );
            }
        }
    }
}
