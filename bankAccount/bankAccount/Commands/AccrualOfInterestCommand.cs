using MediatR;

namespace bankAccount.Commands
{
    public record AccrualOfInterestCommand(Guid Id) : IRequest;
}
