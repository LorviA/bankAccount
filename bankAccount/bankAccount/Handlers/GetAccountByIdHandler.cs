using bankAccount.Data;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountByIdHandler(AccountRepository accountRepository) : IRequestHandler<GetAccountByIdQuery, Account>
    {
        private readonly AccountRepository _accountRepository = accountRepository;
        public async Task<Account> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken) =>
        await _accountRepository.GetProductById(request.Id);
    }
}
