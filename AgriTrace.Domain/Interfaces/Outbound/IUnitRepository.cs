using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IUnitRepository
    : IRepository<Unit, Guid>
{
    Task<Unit?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}