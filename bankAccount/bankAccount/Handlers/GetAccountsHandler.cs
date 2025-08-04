using bankAccount.Data;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;
using System.Security.Principal;

namespace bankAccount.Handlers
{
    public class GetAccountsHandler(AccountRepository accountRepository) : IRequestHandler<GetAccountsQuery, MbResult<IEnumerable<Account>>>
    {
        private readonly AccountRepository _accountRepository = accountRepository;

        public async Task<MbResult<IEnumerable<Account>>> Handle(
            GetAccountsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await _accountRepository.GetAllProducts();
                return MbResult<IEnumerable<Account>>.Success(accounts);
            }
            catch (Exception ex)
            {
                return MbResult<IEnumerable<Account>>.Failure(
                    MbError.Internal($"Failed to retrieve accounts: {ex.Message}")
                );
            }
        }
    }
}
