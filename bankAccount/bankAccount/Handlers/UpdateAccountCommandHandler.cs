using bankAccount.Commands;
using bankAccount.Data;
using bankAccount.Models;
using MediatR;

namespace bankAccount.Handlers
{
    public class UpdateAccountCommandHandler(AccountRepository accountRepository) : IRequestHandler<UpdateAccountCommand, Account>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<Account> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            await _accountRepository.UpdateProdict(request.Id, request.Accont);

            return request.Accont;
        }
    }
}
