using bankAccount.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace bankAccount.Services
{
    public class OutboxHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
    {
        // ReSharper disable once ReplaceWithPrimaryConstructorParameter
        private readonly ApplicationDbContext _dbContext = dbContext;
        private const int WarningThreshold = 100;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _dbContext.OutboxMessages
                    .CountAsync(m => !false, cancellationToken);

                return count > WarningThreshold
                    ? HealthCheckResult.Degraded($"Outbox backlog: {count} messages")
                    : HealthCheckResult.Healthy($"Outbox backlog: {count} messages");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Outbox check failed", ex);
            }
        }
    }
}
