using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record AddAccountCommand(Account Accont): IRequest<Account>;
}
