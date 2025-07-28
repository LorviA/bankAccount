using bankAccount.Data;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountsHandler(AccountRepository accountRepository) : IRequestHandler<GetAccountsQuery, IEnumerable<Account>>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<IEnumerable<Account>> Handle(GetAccountsQuery request,
            CancellationToken cancellationToken) => await _accountRepository.GetAllProducts();
    }
}
