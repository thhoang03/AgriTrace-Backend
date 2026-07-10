using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IBatchRepository
    : IRepository<Batch, Guid>
{
    Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default);
}