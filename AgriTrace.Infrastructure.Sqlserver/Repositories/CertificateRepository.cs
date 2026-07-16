using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class CertificateRepository
    : ICertificateRepository
{

    private readonly ApplicationDbContext _context;



    public CertificateRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }







    public async Task<Certificate> AddAsync(
        Certificate entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            ToModel(entity);



        await _context.Certificates
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
            await _context.Certificates
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
            return;



        _context.Certificates
            .Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<IReadOnlyList<Certificate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Certificates
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }







    public async Task<Certificate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Certificates
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        return model == null
            ? null
            : ToEntity(model);

    }







    public async Task<PagedResult<Certificate>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query =
            _context.Certificates
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



        return new PagedResult<Certificate>(
            models
            .Select(ToEntity)
            .ToList(),

            totalCount,

            pageNumber,

            pageSize);

    }







    public async Task UpdateAsync(
        Certificate entity,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Certificates
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);



        if (model == null)
            return;



        model.CertificateType =
            entity.CertificateType;



        model.FileUrl =
            entity.FileUrl;



        model.IssuedDate =
            entity.IssuedDate;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

    }







    public async Task<IReadOnlyList<Certificate>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Certificates
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







    public async Task<IReadOnlyList<Certificate>> GetByInspectionAsync(
        Guid inspectionId,
        CancellationToken cancellationToken = default)
    {

        var models =
            await _context.Certificates
            .Where(
                x => x.InspectionId == inspectionId)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(
                cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }








    private static Certificate ToEntity(
        CertificateDataModel model)
    {

        return new Certificate(

            model.Id,

            model.BatchId,

            model.InspectionId,

            model.CertificateType ?? string.Empty,

            model.FileUrl ?? string.Empty,

            model.IssuedDate,

            model.CreatedAt,

            model.UpdatedAt

        );

    }








    private static CertificateDataModel ToModel(
        Certificate entity)
    {

        return new CertificateDataModel
        {

            Id =
                entity.Id,


            BatchId =
                entity.BatchId,


            InspectionId =
                entity.InspectionId,


            CertificateType =
                entity.CertificateType,


            FileUrl =
                entity.FileUrl,


            IssuedDate =
                entity.IssuedDate,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}