using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IRecallRepository
    : IRepository<Recall, Guid>
{
    Task<IReadOnlyList<Recall>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Recall>> GetBySeverityAsync(
     RecallSeverity severity,
     CancellationToken cancellationToken = default);
}