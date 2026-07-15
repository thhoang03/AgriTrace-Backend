    using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IOrganizationService
    : IService<Organization, Guid>
{
    Task<IReadOnlyList<Organization>> GetByTypeAsync(
        Guid organizationTypeId,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}   