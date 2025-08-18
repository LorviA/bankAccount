using bankAccount.Commands;
using MediatR;
using bankAccount.Data;
using Microsoft.EntityFrameworkCore;

namespace bankAccount.Controllers
{
    public interface IInterestAccrualService
    {
        Task AccrueInterestForAllAccountsAsync();
    }

    public class InterestAccrualService(IMediator mediator, ApplicationDbContext context) : IInterestAccrualService
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly IMediator _mediator = mediator;
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly ApplicationDbContext _context = context;

        public async Task AccrueInterestForAllAccountsAsync()
        {
            var accountIds = await _context.Accounts
                .Select(a => a.Id).ToListAsync();

            foreach (var accountId in accountIds)
            {
                await _mediator.Send(new AccrualOfInterestCommand(accountId));
            }
        }
    }
}
