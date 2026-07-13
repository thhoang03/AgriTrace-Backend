using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ICertificateService
    : IService<Certificate, Guid>
{
    Task<IReadOnlyList<Certificate>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Certificate>> GetByInspectionAsync(
        Guid inspectionId,
        CancellationToken cancellationToken = default);
}