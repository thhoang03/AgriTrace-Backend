using AgriTrace.Domain.Entities;

public interface IEventTypeRepository
    : IRepository<EventType, Guid>
{
    Task<EventType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}