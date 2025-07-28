using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class AddAccountHandler(AccountRepository accountRepository) : IRequestHandler<AddAccountCommand, Account>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<Account> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            await _accountRepository.AddProduct(request.Accont);

            return request.Accont;
        }
    }
}
