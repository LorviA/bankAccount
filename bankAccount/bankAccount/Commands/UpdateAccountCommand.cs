using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    // ReSharper disable once IdentifierTypo
    public record UpdateAccountCommand(Guid Id, Account Accont) : IRequest<MbResult<Account>>;
}
