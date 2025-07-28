using MediatR;

namespace bankAccount.Abstractions
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
