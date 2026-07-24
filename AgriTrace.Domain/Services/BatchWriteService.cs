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
