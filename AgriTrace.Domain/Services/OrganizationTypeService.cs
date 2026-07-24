using AgriTrace.Domain.Common;
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
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class OrganizationTypeService
    : IOrganizationTypeService
{

    private readonly IOrganizationTypeRepository _repository;


    public OrganizationTypeService(
        IOrganizationTypeRepository repository)
    {
        _repository = repository;
    }



    public async Task<OrganizationType> CreateAsync(
        OrganizationType entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(
            entity,
            cancellationToken);
    }



    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(
            id,
            cancellationToken);
    }



    public async Task<IReadOnlyList<OrganizationType>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }



    public async Task<OrganizationType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }



    public async Task<OrganizationType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Organization type code is required.");
        }


        return await _repository.GetByCodeAsync(
            code.Trim(),
            cancellationToken);
    }



    public async Task<PagedResult<OrganizationType>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }



    public async Task UpdateAsync(
        OrganizationType entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }

}
