using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories;

public class EventRequestRepository : IEventRequestRepository
{
    private readonly ApplicationDbContext _context;

    public EventRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EventRequest> AddAsync(EventRequest entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        await _context.EventRequests.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(EventRequest entity, CancellationToken cancellationToken = default)
    {
        var model = await _context.EventRequests.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (model == null) return;

        model.Status = entity.Status;
        model.RejectionReason = entity.RejectionReason;
        model.ReviewedAt = entity.ReviewedAt;
        model.ReviewedByUserId = entity.ReviewedByUserId;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await _context.EventRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (model == null) return;

        _context.EventRequests.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await _context.EventRequests
            .Include(x => x.Batch)
            .Include(x => x.EventType)
            .Include(x => x.Organization)
            .Include(x => x.RequestedByUser)
            .Include(x => x.ReviewedByUser)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<IReadOnlyList<EventRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var models = await _context.EventRequests
            .Include(x => x.Batch)
            .Include(x => x.EventType)
            .Include(x => x.Organization)
            .Include(x => x.RequestedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<PagedResult<EventRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(pageNumber, pageSize, null, null, null, null, cancellationToken);
    }

    public async Task<IReadOnlyList<EventRequest>> GetByBatchAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var models = await _context.EventRequests
            .Include(x => x.Batch)
            .Include(x => x.EventType)
            .Include(x => x.Organization)
            .Include(x => x.RequestedByUser)
            .Where(x => x.BatchId == batchId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<PagedResult<EventRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? batchId,
        EventRequestStatus? status,
        Guid? organizationId,
        Guid? requestedByUserId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EventRequests
            .Include(x => x.Batch)
            .Include(x => x.EventType)
            .Include(x => x.Organization)
            .Include(x => x.RequestedByUser)
            .Include(x => x.ReviewedByUser)
            .AsQueryable();

        if (batchId.HasValue && batchId.Value != Guid.Empty)
            query = query.Where(x => x.BatchId == batchId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (organizationId.HasValue && organizationId.Value != Guid.Empty)
            query = query.Where(x => x.OrganizationId == organizationId.Value);

        if (requestedByUserId.HasValue && requestedByUserId.Value != Guid.Empty)
            query = query.Where(x => x.RequestedByUserId == requestedByUserId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var entities = models.Select(ToEntity).ToList();

        return new PagedResult<EventRequest>(entities, totalCount, pageNumber, pageSize);
    }

    private static EventRequest ToEntity(EventRequestDataModel model)
    {
        AgriTrace.Domain.Entities.Batches.Batch? batch = null;
        if (model.Batch != null)
        {
            batch = AgriTrace.Domain.Entities.Batches.Batch.Rehydrate(
                id: model.Batch.Id,
                productId: model.Batch.ProductId,
                batchCode: model.Batch.BatchCode,
                quantity: model.Batch.Quantity,
                remainingQuantity: model.Batch.RemainingQuantity,
                sourceQuantity: model.Batch.SourceQuantity,
                unitId: model.Batch.UnitId,
                productionDate: model.Batch.ProductionDate,
                expiryDate: model.Batch.ExpiryDate,
                status: model.Batch.Status,
                currentOrganizationId: model.Batch.CurrentOrganizationId,
                qrCode: model.Batch.QRCode,
                parentBatchId: model.Batch.ParentBatchId,
                rootBatchId: model.Batch.RootBatchId,
                splitId: model.Batch.SplitId,
                createdAt: model.Batch.CreatedAt,
                updatedAt: model.Batch.UpdatedAt
            );
        }

        EventType? eventType = null;
        if (model.EventType != null)
        {
            eventType = new EventType(model.EventType.Code, model.EventType.Name);
            typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))?.SetValue(eventType, model.EventType.Id);
        }

        Organization? organization = null;
        if (model.Organization != null)
        {
            organization = new Organization(
                model.Organization.OrganizationTypeId,
                model.Organization.Name,
                model.Organization.Address
            );
            typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))?.SetValue(organization, model.Organization.Id);
        }

        User? requestedByUser = null;
        if (model.RequestedByUser != null)
        {
            requestedByUser = new User(
                model.RequestedByUser.OrganizationId,
                model.RequestedByUser.FullName,
                model.RequestedByUser.Email,
                model.RequestedByUser.PasswordHash ?? "",
                model.RequestedByUser.Role
            );
            typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))?.SetValue(requestedByUser, model.RequestedByUser.Id);
        }

        return EventRequest.Rehydrate(
            model.Id,
            model.BatchId,
            model.EventTypeId,
            model.OrganizationId,
            model.RequestedByUserId,
            model.EventData,
            model.Location,
            model.Description,
            model.Status,
            model.RejectionReason,
            model.CreatedAt,
            model.ReviewedAt,
            model.ReviewedByUserId,
            batch: batch,
            eventType: eventType,
            organization: organization,
            requestedByUser: requestedByUser
        );
    }

    private static EventRequestDataModel ToModel(EventRequest entity)
    {
        return new EventRequestDataModel
        {
            Id = entity.Id,
            BatchId = entity.BatchId,
            EventTypeId = entity.EventTypeId,
            OrganizationId = entity.OrganizationId,
            RequestedByUserId = entity.RequestedByUserId,
            EventData = entity.EventData,
            Location = entity.Location,
            Description = entity.Description,
            Status = entity.Status,
            RejectionReason = entity.RejectionReason,
            ReviewedAt = entity.ReviewedAt,
            ReviewedByUserId = entity.ReviewedByUserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
