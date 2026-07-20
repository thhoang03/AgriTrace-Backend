using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ISupplyChainEventReadService
{
    Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SupplyChainEvent>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
