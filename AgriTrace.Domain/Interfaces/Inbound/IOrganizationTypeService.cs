using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IOrganizationTypeService
    : IService<OrganizationType, int>
{

    Task<OrganizationType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

}