using bankAccount.Models;
using MediatR;

namespace bankAccount.Queries1
{
    public record GetAccountByIdQuery(Guid Id): IRequest<Account>;
}
