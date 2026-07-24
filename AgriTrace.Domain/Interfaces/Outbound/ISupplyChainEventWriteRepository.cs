using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;

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

