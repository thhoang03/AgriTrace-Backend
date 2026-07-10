using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IQualityInspectionService
    : IService<QualityInspection, Guid>
{
    Task<IReadOnlyList<QualityInspection>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<QualityInspection>> GetByInspectorAsync(
        Guid inspectorId,
        CancellationToken cancellationToken = default);
}