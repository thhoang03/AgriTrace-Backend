using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class BatchService : IBatchService
{
    private readonly IBatchRepository _batchRepository;


    public BatchService(
        IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }


    public async Task<Batch> CreateAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.AddAsync(
            entity,
            cancellationToken);
    }



    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _batchRepository.DeleteAsync(
            id,
            cancellationToken);
    }



    public async Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetAllAsync(
            cancellationToken);
    }



    public async Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByIdAsync(
            id,
            cancellationToken);
    }



    public async Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByBatchCodeAsync(
            batchCode,
            cancellationToken);
    }



    public async Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);
    }



    public async Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByParentBatchAsync(
            parentBatchId,
            cancellationToken);
    }



    public async Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByProductAsync(
            productId,
            cancellationToken);
    }



    public async Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetByRootBatchAsync(
            rootBatchId,
            cancellationToken);
    }



    public async Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _batchRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }



    public async Task UpdateAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {
        await _batchRepository.UpdateAsync(
            entity,
            cancellationToken);
    }
}