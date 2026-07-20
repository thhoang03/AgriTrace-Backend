using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IEventService
{

    Task<SupplyChainEvent> CreateEventAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);



    Task<SupplyChainEvent?> GetByIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);



    Task<bool> VerifyHashChainAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

}