using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.QualityInspections;

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

    Task<PagedResult<QualityInspection>> GetPagedByOrganizationAsync(
        Guid? organizationId,
        int pageNumber,
        int pageSize,
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
