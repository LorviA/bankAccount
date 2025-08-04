using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record CreateTransactionCommand(Guid Id, Transaction Transaction) : IRequest<Account>;
}
