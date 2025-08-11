using bankAccount.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests
{
    public abstract class BaseIntegrationTest: IClassFixture<IntegrationTestAppFactory>
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly ApplicationDbContext DbContext;
        protected BaseIntegrationTest(IntegrationTestAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}
