using MediatR;

namespace bankAccount.Queries1
{
    public record GetBalanceQuery(Guid Id) : IRequest<MbResult<decimal?>>;
}
