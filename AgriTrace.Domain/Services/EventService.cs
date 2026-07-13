using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class EventService : IEventService
{


    private readonly IEventRepository _repository;

    private readonly IHashChainService _hashService;




    public EventService(
        IEventRepository repository,
        IHashChainService hashService)
    {
        _repository = repository;
        _hashService = hashService;
    }





    public async Task<SupplyChainEvent> CreateEventAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {


        var lastEvent =
            await _repository.GetLastEventByBatchAsync(
                entity.BatchId,
                cancellationToken);



        var previousHash =
            lastEvent?.CurrentHash
            ?? "GENESIS";



        var currentHash =
            _hashService.ComputeHash(
                previousHash,
                entity.EventData);



        entity.SetHash(
            previousHash,
            currentHash);



        return await _repository.AddAsync(
            entity,
            cancellationToken);

    }





    public async Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        return await _repository.GetByBatchAsync(
            batchId,
            cancellationToken);

    }





    public async Task<bool> VerifyHashChainAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {


        var events =
            await _repository.GetByBatchAsync(
                batchId,
                cancellationToken);



        string previousHash = "GENESIS";



        foreach (var item in events)
        {

            var hash =
                _hashService.ComputeHash(
                    previousHash,
                    item.EventData);



            if (hash != item.CurrentHash)
            {
                return false;
            }



            previousHash =
                item.CurrentHash;

        }



        return true;

    }

}