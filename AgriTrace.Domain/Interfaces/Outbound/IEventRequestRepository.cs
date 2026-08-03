using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Enums;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IEventRequestRepository : IRepository<EventRequest, Guid>
{
    Task<IReadOnlyList<EventRequest>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<EventRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? batchId,
        EventRequestStatus? status,
        Guid? organizationId,
        Guid? requestedByUserId,
        CancellationToken cancellationToken = default);
}
