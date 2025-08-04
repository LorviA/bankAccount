using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record DeleteAccountCommand(Guid Id): IRequest<MbResult<Account>>;
}
