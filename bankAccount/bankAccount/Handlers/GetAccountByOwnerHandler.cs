using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountByOwnerHandler: IRequestHandler<GetAccountByOwnerQuery, MbResult<IEnumerable<Account>>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByOwnerHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<MbResult<IEnumerable<Account>>> Handle(
            GetAccountByOwnerQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await _accountRepository.GetProductByOwner(request.OwnerId);

                if (accounts == null || !accounts.Any())
                {
                    return MbResult<IEnumerable<Account>>.Failure(
                        MbError.NotFound($"No accounts found for owner ID: {request.OwnerId}")
                    );
                }

                return MbResult<IEnumerable<Account>>.Success(accounts);
            }
            catch (Exception ex)
            {
                return MbResult<IEnumerable<Account>>.Failure(
                    MbError.Internal($"Error retrieving accounts: {ex.Message}")
                );
            }
        }
    }
}
