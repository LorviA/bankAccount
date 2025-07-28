using bankAccount.Models;
using MediatR;

namespace bankAccount.Queries1
{
    public record GetAccountsQuery() : IRequest<IEnumerable<Account>>;
}
