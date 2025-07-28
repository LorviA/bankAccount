using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record UpdateAccountCommand(Guid Id, Account Accont) : IRequest<Account>;
}
