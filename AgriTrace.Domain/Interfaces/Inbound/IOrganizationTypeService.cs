using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IOrganizationTypeService
    : IService<OrganizationType, Guid>
{

    Task<OrganizationType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

}