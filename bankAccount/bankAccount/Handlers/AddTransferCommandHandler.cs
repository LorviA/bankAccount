using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddTransferCommandHandler(AccountRepository accountRepository) : IRequestHandler<AddTransferCommand, IEnumerable<Account>>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<IEnumerable<Account>> Handle(AddTransferCommand request, CancellationToken cancellationToken)
        {
            return await _accountRepository.AddTransfer(request.Transfer);
        }
    }
}
