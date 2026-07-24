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
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class OrganizationTypeRepository
    : IOrganizationTypeRepository
{

    private readonly ApplicationDbContext _context;


    public OrganizationTypeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // ===============================
    // GET BY ID
    // ===============================

    public async Task<OrganizationType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.OrganizationTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }

    // ===============================
    // GET ALL
    // ===============================

    public async Task<IReadOnlyList<OrganizationType>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.OrganizationTypes
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }


    // ===============================
    // PAGING
    // ===============================

    public async Task<PagedResult<OrganizationType>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query = _context.OrganizationTypes
            .AsQueryable();



        var totalCount = await query
            .CountAsync(cancellationToken);



        var models = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);



        var entities = models
            .Select(ToEntity)
            .ToList();



        return new PagedResult<OrganizationType>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }


    // ===============================
    // ADD
    // ===============================

    public async Task<OrganizationType> AddAsync(
        OrganizationType entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.OrganizationTypes
            .AddAsync(
                model,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return entity;

    }

    // ===============================
    // UPDATE
    // ===============================

    public async Task UpdateAsync(
        OrganizationType entity,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.OrganizationTypes
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.Code = entity.Code;

        model.Name = entity.Name;

        model.Description = entity.Description;

        model.UpdatedAt = DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }

    // ===============================
    // DELETE
    // ===============================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.OrganizationTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.OrganizationTypes
            .Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }

    // ===============================
    // GET BY CODE
    // ===============================

    public async Task<OrganizationType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.OrganizationTypes
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }

    // ===============================
    // MAPPING
    // ===============================


    private static OrganizationType ToEntity(
        OrganizationTypeDataModel model)
    {

        return new OrganizationType(
            model.Code,
            model.Name,
            model.Description);

    }
    private static OrganizationTypeDataModel ToModel(
        OrganizationType entity)
    {

        return new OrganizationTypeDataModel
        {
            Id = entity.Id,

            Code = entity.Code,

            Name = entity.Name,

            Description = entity.Description,

            CreatedAt = entity.CreatedAt,

            UpdatedAt = entity.UpdatedAt
        };

    }

}
