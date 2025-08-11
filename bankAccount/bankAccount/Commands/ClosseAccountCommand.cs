using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record CloseAccountCommand(Guid Id) : IRequest<MbResult<Account>>;
}
