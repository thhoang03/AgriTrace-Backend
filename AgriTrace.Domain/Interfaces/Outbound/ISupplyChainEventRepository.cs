using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Outbound;


public interface ISupplyChainEventRepository
    : IRepository<SupplyChainEvent, Guid>
{

    Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken = default);



    Task<SupplyChainEvent?> GetLatestEventAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

}