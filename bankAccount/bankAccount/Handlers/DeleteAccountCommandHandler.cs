using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class DeleteAccountCommandHandler(AccountRepository accountRepository): IRequestHandler<DeleteAccountCommand, Account>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<Account> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
           var result = await _accountRepository.DeleteProduct(request.Id);

            return result;
        }
    }
}
