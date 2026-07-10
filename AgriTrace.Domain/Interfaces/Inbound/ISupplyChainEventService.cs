using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ISupplyChainEventService
    : IService<SupplyChainEvent, Guid>
{
    Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        int eventTypeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplyChainEvent>> GetTimelineAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);
}