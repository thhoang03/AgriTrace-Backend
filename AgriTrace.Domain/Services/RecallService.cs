using AgriTrace.Domain.Common;
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

public class RecallService : IRecallService
{
    private readonly IRecallRepository _repository;

    public RecallService(
        IRecallRepository repository)
    {
        _repository = repository;
    }

    public async Task<Recall> CreateAsync(
        Recall entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        Recall entity,
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

    public async Task<Recall?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Recall>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedResult<Recall>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Recall>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByBatchAsync(
            batchId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Recall>> GetBySeverityAsync(
        RecallSeverity severity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetBySeverityAsync(
            severity,
            cancellationToken);
    }
}
