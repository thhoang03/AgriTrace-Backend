using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IOrganizationService
    : IService<Organization, Guid>
{
    Task<IReadOnlyList<Organization>> GetByTypeAsync(
        int organizationTypeId,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}