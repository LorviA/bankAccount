using bankAccount.Models;
using MediatR;

namespace bankAccount.Queries1
{
    public record GetAccountByOwnerQuery(Guid OwnerId) : IRequest<MbResult<IEnumerable<Account>>>;
}
