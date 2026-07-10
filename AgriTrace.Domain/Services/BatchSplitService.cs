using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public class BatchSplitService : IBatchSplitService
{
    private readonly IBatchSplitRepository _repository;

    public BatchSplitService(
        IBatchSplitRepository repository)
    {
        _repository = repository;
    }

    public async Task<BatchSplit> CreateAsync(
        BatchSplit entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        BatchSplit entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<BatchSplit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<BatchSplit>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedResult<BatchSplit>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public async Task<IReadOnlyList<BatchSplit>> GetBySourceBatchAsync(
        Guid sourceBatchId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetBySourceBatchAsync(
            sourceBatchId,
            cancellationToken);
    }
}