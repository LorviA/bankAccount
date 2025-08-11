using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    // ReSharper disable once IdentifierTypo
    public record AddAccountCommand(Account Accont): IRequest<MbResult<Account>>;
}
