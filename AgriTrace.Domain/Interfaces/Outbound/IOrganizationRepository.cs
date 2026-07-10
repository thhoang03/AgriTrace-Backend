using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IOrganizationRepository
    : IRepository<Organization, Guid>
{
    Task<IReadOnlyList<Organization>> GetByTypeAsync(
        Guid organizationTypeId,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}