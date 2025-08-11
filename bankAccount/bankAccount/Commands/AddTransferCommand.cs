using bankAccount.Models;
using MediatR;

namespace bankAccount.Commands
{
    public record AddTransferCommand(Transaction Transfer): IRequest<MbResult<IEnumerable<Account>>?>;
}
