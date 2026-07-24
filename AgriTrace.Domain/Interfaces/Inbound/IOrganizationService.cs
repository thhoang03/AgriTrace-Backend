    using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;

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
