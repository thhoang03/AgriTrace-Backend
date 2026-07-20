using AgriTrace.Domain.Entities;

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
