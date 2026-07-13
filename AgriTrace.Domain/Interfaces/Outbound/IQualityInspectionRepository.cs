using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Outbound;


public interface IQualityInspectionRepository
    : IRepository<QualityInspection, Guid>
{

    Task<IReadOnlyList<QualityInspection>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<QualityInspection>> GetByInspectorAsync(
        Guid inspectorId,
        CancellationToken cancellationToken = default);

}