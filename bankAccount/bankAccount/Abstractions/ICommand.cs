using MediatR;

namespace bankAccount.Abstractions
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
