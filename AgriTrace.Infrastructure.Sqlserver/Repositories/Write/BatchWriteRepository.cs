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


namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Write;


public sealed class BatchWriteRepository
    : IBatchWriteRepository
{

    private readonly ApplicationDbContext _context;


    public BatchWriteRepository(
        ApplicationDbContext context)
    {
        _context = context;
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
        {
            throw new NotFoundException($"Batch with id '{entity.Id}' was not found.");
        }




        model.BatchCode =
            entity.BatchCode;



        model.Quantity =
            entity.Quantity;



        model.RemainingQuantity =
            entity.RemainingQuantity;



        model.ProductionDate =
            entity.ProductionDate;



        model.ExpiryDate =
            entity.ExpiryDate;



        model.Status =
            entity.Status;



        model.CurrentOrganizationId =
            entity.CurrentOrganizationId;



        model.ParentBatchId =
            entity.ParentBatchId;



        model.RootBatchId =
            entity.RootBatchId;



        model.SplitId =
            entity.SplitId;



        model.SourceQuantity =
            entity.SourceQuantity;



        model.QRCode =
            entity.QRCode;




        model.UpdatedAt =
            DateTime.UtcNow;




        await _context.SaveChangesAsync(
            cancellationToken);

    }









    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var model =
            await _context.Batches

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (model == null)
        {
            throw new NotFoundException($"Batch with id '{id}' was not found.");
        }



        _context.Batches.Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

    }









    private static Batch ToEntity(
        BatchDataModel model)
    {
        return Batch.Rehydrate(
            id:                    model.Id,
            productId:             model.ProductId,
            batchCode:             model.BatchCode,
            quantity:              model.Quantity,
            remainingQuantity:     model.RemainingQuantity,
            sourceQuantity:        model.SourceQuantity,
            unitId:                model.UnitId,
            productionDate:        model.ProductionDate,
            expiryDate:            model.ExpiryDate,
            status:                model.Status,
            currentOrganizationId: model.CurrentOrganizationId,
            qrCode:                model.QRCode,
            parentBatchId:         model.ParentBatchId,
            rootBatchId:           model.RootBatchId,
            splitId:               model.SplitId,
            createdAt:             model.CreatedAt,
            updatedAt:             model.UpdatedAt);
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



            CurrentOrganizationId =
                entity.CurrentOrganizationId,



            UnitId =
                entity.UnitId,



            BatchCode =
                entity.BatchCode,



            Quantity =
                entity.Quantity,



            RemainingQuantity =
                entity.RemainingQuantity,



            ProductionDate =
                entity.ProductionDate,



            ExpiryDate =
                entity.ExpiryDate,



            Status =
                entity.Status,



            QRCode =
                entity.QRCode,



            ParentBatchId =
                entity.ParentBatchId,



            RootBatchId =
                entity.RootBatchId,



            SplitId =
                entity.SplitId,



            SourceQuantity =
                entity.SourceQuantity,



            CreatedAt =
                entity.CreatedAt,



            UpdatedAt =
                entity.UpdatedAt

        };

    }

}
