using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class CreateTransactionCommandHandler(AccountRepository accountRepository) : IRequestHandler<CreateTransactionCommand, Account>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<Account> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
           return await _accountRepository.CreateTransaction(request.Id, request.Transaction);
        }
    }
}
