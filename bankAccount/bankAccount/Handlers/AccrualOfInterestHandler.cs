using bankAccount.Commands;
using bankAccount.Interfaces;
using MediatR;

namespace bankAccount.Handlers
{
    public class AccrualOfInterestHandler(IAccountRepository accountRepository) : IRequestHandler<AccrualOfInterestCommand>
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IAccountRepository _accountRepository = accountRepository;

        public async Task Handle(AccrualOfInterestCommand request, CancellationToken cancellationToken)
        {
            await _accountRepository.AccrueInterestAsync(request.Id);
        }
    }
}
