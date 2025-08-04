using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, MbResult<Account>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<MbResult<Account>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetProductById(request.Id);

            if (account == null)
            {
                return MbResult<Account>.Failure(
                    MbError.NotFound($"Account with ID {request.Id} not found")
                );
            }

            return MbResult<Account>.Success(account);
        }
    }
}
