using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Read;


public sealed class BatchReadRepository
    : IBatchReadRepository
{

    private readonly ApplicationDbContext _context;


    public BatchReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }




    public async Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Batches

            .AsNoTracking()

            .Include(x => x.Product)

            .Include(x => x.Unit)

            .Include(x => x.ParentBatch)

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }






    public async Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Batches

            .AsNoTracking()

            .Include(x => x.Product)

            .Include(x => x.Unit)

            .OrderBy(x => x.BatchCode)

            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Batches

            .AsNoTracking()

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

        var models = await _context.Batches

            .AsNoTracking()

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







    public async Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default)
    {

        var model = await _context.Batches

            .AsNoTracking()

            .FirstOrDefaultAsync(
                x => x.BatchCode == batchCode,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }








    public async Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Batches

            .AsNoTracking()

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

        var models = await _context.Batches

            .AsNoTracking()

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









    public async Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.Batches
            .AsNoTracking();



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



        return new PagedResult<Batch>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);

    }









    public async Task<PagedResult<Batch>> SearchAsync(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        IQueryable<BatchDataModel> query =
            _context.Batches
            .AsNoTracking();



        if (productId.HasValue)
        {
            query = query.Where(
                x => x.ProductId == productId.Value);
        }



        if (organizationId.HasValue)
        {
            query = query.Where(
                x => x.CurrentOrganizationId == organizationId.Value);
        }



        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(
                x => x.BatchCode.Contains(search));
        }




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




        return new PagedResult<Batch>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);

    }







    private static Batch ToEntity(
        BatchDataModel model)
    {
        return Batch.Rehydrate(
            id:                   model.Id,
            productId:            model.ProductId,
            batchCode:            model.BatchCode,
            quantity:             model.Quantity,
            remainingQuantity:    model.RemainingQuantity,
            sourceQuantity:       model.SourceQuantity,
            unitId:               model.UnitId,
            productionDate:       model.ProductionDate,
            expiryDate:           model.ExpiryDate,
            status:               model.Status,
            currentOrganizationId: model.CurrentOrganizationId,
            qrCode:               model.QRCode,
            parentBatchId:        model.ParentBatchId,
            rootBatchId:          model.RootBatchId,
            splitId:              model.SplitId,
            createdAt:            model.CreatedAt,
            updatedAt:            model.UpdatedAt);
    }

}