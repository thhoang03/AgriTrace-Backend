using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public class QualityInspectionService : IQualityInspectionService
{
    private readonly IQualityInspectionRepository _repository;

    public QualityInspectionService(
        IQualityInspectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<QualityInspection> CreateAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(
            entity,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(
            id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspection>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }

    public async Task<QualityInspection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspection>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByBatchAsync(
            batchId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspection>> GetByInspectorAsync(
        Guid inspectorId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByInspectorAsync(
            inspectorId,
            cancellationToken);
    }

    public async Task<PagedResult<QualityInspection>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public async Task UpdateAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }
}