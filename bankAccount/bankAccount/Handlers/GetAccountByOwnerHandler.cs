using bankAccount.Data;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountByOwnerHandler(AccountRepository accountRepository) : IRequestHandler<GetAccountByOwnerQuery, IEnumerable<Account>>
    {
        private readonly AccountRepository _accountRepository = accountRepository;
        public async Task<IEnumerable<Account>> Handle(GetAccountByOwnerQuery request, CancellationToken cancellationToken) =>
        await _accountRepository.GetProductByOwner(request.OwnerId);
    }
}
