using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public class BatchMergeService : IBatchMergeService
{
    private readonly IBatchMergeRepository _repository;

    public BatchMergeService(
        IBatchMergeRepository repository)
    {
        _repository = repository;
    }

    public async Task<BatchMerge> CreateAsync(
        BatchMerge entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        BatchMerge entity,
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

    public async Task<BatchMerge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<BatchMerge>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedResult<BatchMerge>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public async Task<IReadOnlyList<BatchMerge>> GetByNewBatchAsync(
        Guid newBatchId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByNewBatchAsync(
            newBatchId,
            cancellationToken);
    }
}