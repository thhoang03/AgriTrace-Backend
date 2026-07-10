using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface ICertificateRepository
    : IRepository<Certificate, Guid>
{
    Task<IReadOnlyList<Certificate>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Certificate>> GetByInspectionAsync(
        Guid inspectionId,
        CancellationToken cancellationToken = default);
}