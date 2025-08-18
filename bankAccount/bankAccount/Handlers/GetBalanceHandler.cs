using bankAccount.Interfaces;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetBalanceHandler(IAccountRepository accountRepository) : IRequestHandler<GetBalanceQuery, MbResult<decimal?>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<decimal?>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetBalance(request.Id);

            if (account == null)
            {
                return MbResult<decimal?>.Failure(
                    MbError.NotFound($"Account with ID {request.Id} not found")
                );
            }

            return MbResult<decimal?>.Success(account);
        }
    }
}
