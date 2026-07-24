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


public class UnitRepository
    : IUnitRepository
{

    private readonly ApplicationDbContext _context;



    public UnitRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }



    // ==========================
    // GET BY ID
    // ==========================

    public async Task<Unit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Units
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

    public async Task<IReadOnlyList<Unit>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Units
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    // ==========================
    // GET PAGED
    // ==========================

    public async Task<PagedResult<Unit>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query = _context.Units
            .AsQueryable();



        var totalCount = await query
            .CountAsync(cancellationToken);



        var models = await query
            .OrderBy(x => x.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);



        var entities = models
            .Select(ToEntity)
            .ToList();



        return new PagedResult<Unit>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }





    // ==========================
    // ADD
    // ==========================

    public async Task<Unit> AddAsync(
        Unit entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.Units
            .AddAsync(
                model,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return entity;

    }





    // ==========================
    // UPDATE
    // ==========================

    public async Task UpdateAsync(
        Unit entity,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Units
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.Code = entity.Code;

        model.Name = entity.Name;

        model.UpdatedAt = DateTime.UtcNow;



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

        var model = await _context.Units
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.Units.Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }





    // ==========================
    // GET BY CODE
    // ==========================

    public async Task<Unit?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Units
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }





    // ==========================
    // GET BY NAME
    // ==========================

    public async Task<Unit?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Units
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

    private static Unit ToEntity(
        UnitDataModel model)
    {

        return new Unit(
            model.Code,
            model.Name);

    }





    private static UnitDataModel ToModel(
        Unit entity)
    {

        return new UnitDataModel
        {
            Id = entity.Id,

            Code = entity.Code,

            Name = entity.Name,

            CreatedAt = entity.CreatedAt,

            UpdatedAt = entity.UpdatedAt
        };

    }

}
