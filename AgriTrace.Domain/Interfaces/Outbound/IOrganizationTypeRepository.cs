using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Outbound;


public interface IOrganizationTypeRepository
    : IRepository<OrganizationType, Guid>
{

    Task<OrganizationType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

}