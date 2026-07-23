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

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ISupplyChainEventWriteService
{
    /// <summary>
    /// Creates a new supply chain event and computes the hash chain automatically.
    /// </summary>
    Task<SupplyChainEvent> CreateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies that the hash chain for a batch is intact (tamper-proof check).
    /// </summary>
    Task<bool> VerifyHashChainAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);
}

