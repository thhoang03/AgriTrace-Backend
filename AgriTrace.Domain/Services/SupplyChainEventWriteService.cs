using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public sealed class SupplyChainEventWriteService : ISupplyChainEventWriteService
{
    private readonly ISupplyChainEventWriteRepository _repository;
    private readonly IHashChainService _hashService;

    public SupplyChainEventWriteService(
        ISupplyChainEventWriteRepository repository,
        IHashChainService hashService)
    {
        _repository = repository;
        _hashService = hashService;
    }

    public async Task<SupplyChainEvent> CreateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {
        var lastEvent = await _repository.GetLastEventByBatchAsync(
            entity.BatchId,
            cancellationToken);

        var previousHash = lastEvent?.CurrentHash ?? "GENESIS";

        var currentHash = _hashService.ComputeHash(previousHash, entity.EventData);

        entity.SetHash(previousHash, currentHash);

        return await _repository.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(entity, cancellationToken);

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);

    public async Task<bool> VerifyHashChainAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        // Re-use the read repository via write repo's GetLastEventByBatchAsync
        // — we need ALL events ordered; inject read repo via interface is cleaner,
        //   but to keep symmetry we resolve events through a dedicated read call.
        // NOTE: VerifyHashChain needs all events in order; this is a cross-concern.
        // We accept a read-side dependency here via constructor injection.
        throw new NotSupportedException(
            "VerifyHashChainAsync should be called via ISupplyChainEventReadService. " +
            "Use VerifyHashChainQuery which injects both read repo and hash service.");
    }
}
