namespace bankAccount.Interfaces
{
    public interface IOutboxService
    {
        Task AddEventAsync(object @event, CancellationToken cancellationToken);
    }
}
