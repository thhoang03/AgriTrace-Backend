using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Outbound;


public interface IEventRepository
    : IRepository<SupplyChainEvent, Guid>
{

    Task<SupplyChainEvent?> GetLastEventByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

}