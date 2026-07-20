using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface ISupplyChainEventWriteRepository
{
    Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SupplyChainEvent?> GetLastEventByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<SupplyChainEvent> AddAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
