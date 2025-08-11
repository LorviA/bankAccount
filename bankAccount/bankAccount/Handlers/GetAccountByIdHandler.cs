using bankAccount.Interfaces;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountByIdHandler(IAccountRepository accountRepository) : IRequestHandler<GetAccountByIdQuery, MbResult<Account>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

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
