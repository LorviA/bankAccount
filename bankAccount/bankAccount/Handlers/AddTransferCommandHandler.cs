using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddTransferCommandHandler : IRequestHandler<AddTransferCommand, MbResult<IEnumerable<Account>>>
    {
        private readonly IAccountRepository _accountRepository;

        public AddTransferCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<MbResult<IEnumerable<Account>>> Handle(
            AddTransferCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _accountRepository.AddTransfer(request.Transfer);

                if (result == null)
                {
                    return MbResult<IEnumerable<Account>>.Failure(
                        MbError.NotFound("Transfer failed: invalid accounts or insufficient funds")
                    );
                }

                return MbResult<IEnumerable<Account>>.Success(result);
            }
            catch (Exception ex)
            {
                return MbResult<IEnumerable<Account>>.Failure(
                    MbError.Internal($"Error processing transfer: {ex.Message}")
                );
            }
        }
    }
}