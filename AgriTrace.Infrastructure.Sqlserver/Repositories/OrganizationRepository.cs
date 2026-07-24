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


public class OrganizationRepository
    : IOrganizationRepository
{

    private readonly ApplicationDbContext _context;


    public OrganizationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }



    // ==========================
    // GET BY ID
    // ==========================

    public async Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Organizations
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }





    // ==========================
    // GET ALL
    // ==========================

    public async Task<IReadOnlyList<Organization>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Organizations
            .Include(x => x.OrganizationType)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    // ==========================
    // GET PAGED
    // ==========================

    public async Task<PagedResult<Organization>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query = _context.Organizations
            .Include(x => x.OrganizationType)
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



        return new PagedResult<Organization>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }





    // ==========================
    // ADD
    // ==========================

    public async Task<Organization> AddAsync(
        Organization entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.Organizations
            .AddAsync(
                model,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        // Tr? l?i entity du?c rehydrate t? model v?a luu d? d?m b?o
        // Id/CreatedAt tr? v? kh?p chính xác v?i d? li?u dã persist.
        return ToEntity(model);

    }





    // ==========================
    // UPDATE
    // ==========================

    public async Task UpdateAsync(
        Organization entity,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Organizations
            .Include(x => x.OrganizationType)
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.OrganizationTypeId =
            entity.OrganizationTypeId;


        model.Name =
            entity.Name;


        model.Address =
            entity.Address;


        model.Status =
            entity.Status;


        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }





    // ==========================
    // DELETE
    // ==========================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Organizations
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.Organizations.Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }





    // ==========================
    // GET BY TYPE
    // ==========================

    public async Task<IReadOnlyList<Organization>> GetByTypeAsync(
        Guid organizationTypeId,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Organizations
            .Include(x => x.OrganizationType)
            .Where(
                x => x.OrganizationTypeId == organizationTypeId)
            .OrderBy(
                x => x.Name)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    // ==========================
    // GET BY NAME
    // ==========================

    public async Task<Organization?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Organizations
            .Include(x => x.OrganizationType)
            .FirstOrDefaultAsync(
                x => x.Name == name,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }





    // ==========================
    // MAPPING
    // ==========================

    private static Organization ToEntity(
        OrganizationDataModel model)
    {

        // Dùng constructor rehydrate d? gi? dúng Id/Status/CreatedAt/UpdatedAt
        // t? database, thay vì constructor "t?o m?i" (s? sinh Id ng?u nhiên
        // và luôn set Status = Active).
        return new Organization(
            model.Id,
            model.OrganizationTypeId,
            model.Name,
            model.Address,
            model.Status,
            model.CreatedAt,
            model.UpdatedAt,
            model.OrganizationType == null
                ? null
                : new OrganizationType(
                    model.OrganizationType.Id,
                    model.OrganizationType.Code,
                    model.OrganizationType.Name,
                    model.OrganizationType.Description,
                    model.OrganizationType.CreatedAt,
                    model.OrganizationType.UpdatedAt));

    }





    private static OrganizationDataModel ToModel(
        Organization entity)
    {

        return new OrganizationDataModel
        {
            Id = entity.Id,

            OrganizationTypeId =
                entity.OrganizationTypeId,

            Name =
                entity.Name,

            Address =
                entity.Address,

            Status =
                entity.Status,

            CreatedAt =
                entity.CreatedAt,

            UpdatedAt =
                entity.UpdatedAt
        };

    }

}
