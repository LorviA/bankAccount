using bankAccount.Interfaces;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;

namespace bankAccount.Handlers
{
    public class GetAccountsHandler(IAccountRepository accountRepository) : IRequestHandler<GetAccountsQuery, MbResult<IEnumerable<Account>>>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

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
