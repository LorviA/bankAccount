using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record CreateTransactionCommand(Transaction Transaction) : IRequest<Account>;
}
