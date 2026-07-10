using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class BatchRepository
    : IBatchRepository
{

    private readonly ApplicationDbContext _context;



    public BatchRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }





    public async Task<Batch> AddAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.Batches
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

        var model = await _context.Batches
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.Batches.Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Batches
            .OrderBy(
                x => x.BatchCode)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Batches
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }







    public async Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.Batches
            .AsQueryable();



        var totalCount =
            await query.CountAsync(
                cancellationToken);



        var models =
            await query
            .OrderBy(
                x => x.BatchCode)
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



        return new PagedResult<Batch>(
            entities,
            totalCount,
            pageNumber,
            pageSize);

    }







    public async Task UpdateAsync(
        Batch entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Batches
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.BatchCode =
            entity.BatchCode;



        model.Quantity =
            entity.Quantity;



        model.Status =
            (int)entity.Status;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Batches
            .FirstOrDefaultAsync(
                x => x.BatchCode == batchCode,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }







    public async Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Batches
            .Where(
                x => x.ProductId == productId)
            .OrderBy(
                x => x.BatchCode)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Batches
            .Where(
                x => x.CurrentOrganizationId == organizationId)
            .OrderBy(
                x => x.BatchCode)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Batches
            .Where(
                x => x.ParentBatchId == parentBatchId)
            .OrderBy(
                x => x.BatchCode)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Batches
            .Where(
                x => x.RootBatchId == rootBatchId)
            .OrderBy(
                x => x.BatchCode)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    private static Batch ToEntity(
        BatchDataModel model)
    {

        var entity = new Batch(
            model.ProductId,
            model.BatchCode,
            model.Quantity,
            model.UnitId,
            model.ProductionDate,
            model.ExpiryDate);



        entity.ChangeStatus(
            (BatchStatus)model.Status);



        return entity;

    }







    private static BatchDataModel ToModel(
        Batch entity)
    {

        return new BatchDataModel
        {

            Id =
                entity.Id,


            ProductId =
                entity.ProductId,


            UnitId =
                entity.UnitId,


            BatchCode =
                entity.BatchCode,


            Quantity =
                entity.Quantity,


            RemainingQuantity =
                entity.Quantity,


            ProductionDate =
                entity.ProductionDate,


            ExpiryDate =
                entity.ExpiryDate,


            Status =
                (int)entity.Status,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}