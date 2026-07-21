using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class RecallRepository
    : IRecallRepository
{

    private readonly ApplicationDbContext _context;



    public RecallRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }




    // ==========================
    // ADD
    // ==========================

    public async Task<Recall> AddAsync(
        Recall entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.Recalls
            .AddAsync(
                model,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return entity;

    }






    // ==========================
    // DELETE
    // ==========================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Recalls
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.Recalls
            .Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    // ==========================
    // GET ALL
    // ==========================

    public async Task<IReadOnlyList<Recall>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Recalls
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    // ==========================
    // GET BY ID
    // ==========================

    public async Task<Recall?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Recalls
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }







    // ==========================
    // PAGING
    // ==========================

    public async Task<PagedResult<Recall>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.Recalls
            .AsQueryable();



        var totalCount =
            await query.CountAsync(
                cancellationToken);



        var models =
            await query
            .OrderByDescending(
                x => x.CreatedAt)
            .Skip(
                (pageNumber - 1) * pageSize)
            .Take(
                pageSize)
            .ToListAsync(
                cancellationToken);



        return new PagedResult<Recall>(

            models
            .Select(ToEntity)
            .ToList(),

            totalCount,

            pageNumber,

            pageSize

        );

    }







    // ==========================
    // UPDATE
    // ==========================

    public async Task UpdateAsync(
        Recall entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Recalls
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.Reason =
            entity.Reason;



        model.Severity =
            (int)entity.Severity;



        model.Status =
            (int)entity.Status;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    // ==========================
    // GET BY BATCH
    // ==========================

    public async Task<IReadOnlyList<Recall>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Recalls
            .Where(
                x => x.BatchId == batchId)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }

    // ==========================
    // GET BY SEVERITY
    // ==========================

    public async Task<IReadOnlyList<Recall>> GetBySeverityAsync(
        RecallSeverity severity,
        CancellationToken cancellationToken = default)
    {
        var models =
            await _context.Recalls
            .Where(
                x => x.Severity == (int)severity)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();
    }





    // ==========================
    // MAPPING
    // ==========================

    private static Recall ToEntity(
        RecallDataModel model)
    {

        return Recall.Rehydrate(

            model.Id,

            model.BatchId,

            model.CreatedBy,

            model.Reason ?? string.Empty,

            (RecallSeverity)model.Severity,

            (RecallStatus)model.Status,

            model.CreatedAt,

            model.UpdatedAt

        );

    }







    private static RecallDataModel ToModel(
        Recall entity)
    {

        return new RecallDataModel
        {

            Id =
                entity.Id,


            BatchId =
                entity.BatchId,


            CreatedBy =
                entity.CreatedBy,


            Reason =
                entity.Reason,


            Severity =
                (int)entity.Severity,


            Status =
                (int)entity.Status,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}