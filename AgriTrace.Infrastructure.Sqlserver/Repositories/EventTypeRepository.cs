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


public class EventTypeRepository
    : IEventTypeRepository
{

    private readonly ApplicationDbContext _context;


    public EventTypeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<EventType> AddAsync(
        EventType entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);


        await _context.EventTypes
            .AddAsync(
                model,
                cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        return entity;
    }




    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.EventTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (model == null)
            return;


        _context.EventTypes.Remove(model);


        await _context.SaveChangesAsync(
            cancellationToken);

    }




    public async Task<IReadOnlyList<EventType>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.EventTypes
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);


        return models
            .Select(ToEntity)
            .ToList();

    }




    public async Task<EventType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.EventTypes
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }





    public async Task<EventType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.EventTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }





    public async Task<PagedResult<EventType>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query = _context.EventTypes
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



        return new PagedResult<EventType>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }





    public async Task UpdateAsync(
        EventType entity,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.EventTypes
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





    private static EventType ToEntity(
        EventTypeDataModel model)
    {

        return new EventType(
            model.Code,
            model.Name);

    }





    private static EventTypeDataModel ToModel(
        EventType entity)
    {

        return new EventTypeDataModel
        {
            Id = entity.Id,

            Code = entity.Code,

            Name = entity.Name,

            CreatedAt = entity.CreatedAt,

            UpdatedAt = entity.UpdatedAt
        };

    }

}
