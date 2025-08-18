using bankAccount.Models;
using MediatR;

namespace bankAccount.Queries1
{
    // ReSharper disable once EmptyConstructor
    public record GetAccountsQuery() : IRequest<MbResult<IEnumerable<Account>>>;
}
