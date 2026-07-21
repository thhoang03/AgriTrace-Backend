using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public sealed class BatchWriteService
    : IBatchWriteService
{

    private readonly IBatchWriteRepository _repository;



    public BatchWriteService(
        IBatchWriteRepository repository)
    {
        _repository = repository;
    }





    public Task<Batch> CreateAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(
            entity,
            cancellationToken);
    }





    public Task UpdateAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(
            entity,
            cancellationToken);
    }





    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(
            id,
            cancellationToken);
    }

}