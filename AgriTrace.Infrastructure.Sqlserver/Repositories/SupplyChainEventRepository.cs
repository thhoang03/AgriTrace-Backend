using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class SupplyChainEventRepository
    : ISupplyChainEventRepository
{

    private readonly ApplicationDbContext _context;



    public SupplyChainEventRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }





    public async Task<SupplyChainEvent> AddAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.SupplyChainEvents
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

        var model =
            await _context.SupplyChainEvents
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.SupplyChainEvents
            .Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }








    public async Task<IReadOnlyList<SupplyChainEvent>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.SupplyChainEvents
            .OrderByDescending(
                x => x.EventTime)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }








    public async Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.SupplyChainEvents
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }








    public async Task<PagedResult<SupplyChainEvent>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.SupplyChainEvents
            .AsQueryable();



        var totalCount =
            await query
            .CountAsync(
                cancellationToken);



        var models =
            await query
            .OrderByDescending(
                x => x.EventTime)
            .Skip(
                (pageNumber - 1) * pageSize)
            .Take(
                pageSize)
            .ToListAsync(
                cancellationToken);



        var entities =
            models
            .Select(ToEntity)
            .ToList();



        return new PagedResult<SupplyChainEvent>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }








    public async Task UpdateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.SupplyChainEvents
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.EventData =
            entity.EventData;



        model.Location =
            entity.Location;



        model.PreviousHash =
            entity.PreviousHash;



        model.CurrentHash =
            entity.CurrentHash;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }








    public async Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.SupplyChainEvents
            .Where(
                x => x.BatchId == batchId)
            .OrderBy(
                x => x.EventTime)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }








    public async Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.SupplyChainEvents
            .Where(
                x => x.EventTypeId == eventTypeId)
            .OrderBy(
                x => x.EventTime)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }








    public async Task<IReadOnlyList<SupplyChainEvent>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.SupplyChainEvents
            .Where(
                x => x.OrganizationId == organizationId)
            .OrderBy(
                x => x.EventTime)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }








    public async Task<SupplyChainEvent?> GetLatestEventAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.SupplyChainEvents
            .Where(
                x => x.BatchId == batchId)
            .OrderByDescending(
                x => x.EventTime)
            .FirstOrDefaultAsync(
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }








    private static SupplyChainEvent ToEntity(
        SupplyChainEventDataModel model)
    {

        return new SupplyChainEvent(

            model.BatchId,

            model.EventTypeId,

            model.OrganizationId,

            model.PerformedByUserId,

            model.EventData,

            model.Location,

            model.PreviousHash,

            model.CurrentHash

        );

    }








    private static SupplyChainEventDataModel ToModel(
        SupplyChainEvent entity)
    {

        return new SupplyChainEventDataModel
        {

            Id =
                entity.Id,



            BatchId =
                entity.BatchId,



            EventTypeId =
                entity.EventTypeId,



            OrganizationId =
                entity.OrganizationId,



            PerformedByUserId =
                entity.PerformedByUserId,



            EventData =
                entity.EventData,



            Location =
                entity.Location,



            PreviousHash =
                entity.PreviousHash,



            CurrentHash =
                entity.CurrentHash,



            EventTime =
                entity.EventTime,



            CreatedAt =
                entity.CreatedAt,



            UpdatedAt =
                entity.UpdatedAt

        };

    }

}