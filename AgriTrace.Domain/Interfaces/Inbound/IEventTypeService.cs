using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IEventTypeService
    : IService<EventType, int>
{
    Task<EventType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}