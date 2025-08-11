using bankAccount.Commands;
using bankAccount.Interfaces;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddTransferCommandHandler(IAccountRepository accountRepository) : IRequestHandler<AddTransferCommand, MbResult<IEnumerable<Account>>?>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<IEnumerable<Account>>?> Handle(
            AddTransferCommand request,
            CancellationToken cancellationToken)
        {
            return await _accountRepository.AddTransfer(request.Transfer);
        }
    }
}