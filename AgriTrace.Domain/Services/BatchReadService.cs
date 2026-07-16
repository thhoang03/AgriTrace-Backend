using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public sealed class BatchReadService
    : IBatchReadService
{

    private readonly IBatchReadRepository _repository;


    public BatchReadService(
        IBatchReadRepository repository)
    {
        _repository = repository;
    }



    public Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(
            id,
            cancellationToken);



    public Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(
            cancellationToken);



    public Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default)
        => _repository.GetByBatchCodeAsync(
            batchCode,
            cancellationToken);



    public Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
        => _repository.GetByProductAsync(
            productId,
            cancellationToken);



    public Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
        => _repository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);



    public Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default)
        => _repository.GetByParentBatchAsync(
            parentBatchId,
            cancellationToken);



    public Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default)
        => _repository.GetByRootBatchAsync(
            rootBatchId,
            cancellationToken);



    public Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);



    public Task<PagedResult<Batch>> SearchAsync(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => _repository.SearchAsync(
            productId,
            organizationId,
            search,
            pageNumber,
            pageSize,
            cancellationToken);

}