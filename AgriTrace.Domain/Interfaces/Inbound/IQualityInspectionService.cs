using AgriTrace.Domain.Entities.QualityInspections;

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

    Task<QualityInspection?> GetByIdWithLabTestsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InspectionLabTest?> GetLabTestByIdAsync(
        Guid labTestId,
        CancellationToken cancellationToken = default);

    Task DeleteLabTestAsync(
        Guid labTestId,
        CancellationToken cancellationToken = default);

    Task AddLabTestAsync(
        InspectionLabTest labTest,
        CancellationToken cancellationToken = default);
}
