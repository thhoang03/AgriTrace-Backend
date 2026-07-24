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


public class QualityInspectionRepository
    : IQualityInspectionRepository
{

    private readonly ApplicationDbContext _context;



    public QualityInspectionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }





    public async Task<QualityInspection> AddAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);


        await _context.QualityInspections
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
            await _context.QualityInspections
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.QualityInspections
            .Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<IReadOnlyList<QualityInspection>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.QualityInspections
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<QualityInspection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.QualityInspections
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }







    public async Task<PagedResult<QualityInspection>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.QualityInspections
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



        return new PagedResult<QualityInspection>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);

    }







    public async Task UpdateAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.QualityInspections
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.Status =
            entity.Status;



        model.Result =
            entity.Result;



        model.Notes =
            entity.Notes;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<IReadOnlyList<QualityInspection>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.QualityInspections
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







    public async Task<IReadOnlyList<QualityInspection>> GetByInspectorAsync(
        Guid inspectorId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.QualityInspections
            .Where(
                x => x.InspectorId == inspectorId)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    private static QualityInspection ToEntity(
        QualityInspectionDataModel model)
    {

        return new QualityInspection(

            model.Id,

            model.BatchId,

            model.InspectorId,

            (InspectionStatus)model.Status,

            model.Result,

            model.Notes,

            model.CreatedAt,

            model.UpdatedAt

        );

    }


    private static QualityInspectionDataModel ToModel(
        QualityInspection entity)
    {

        return new QualityInspectionDataModel
        {

            Id =
                entity.Id,


            BatchId =
                entity.BatchId,


            InspectorId =
                entity.InspectorId,


            Status =
                entity.Status,


            Result =
                entity.Result,


            Notes =
                entity.Notes,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}
