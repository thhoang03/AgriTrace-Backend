using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IUnitService
    : IService<Unit, Guid>
{
    Task<Unit?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}