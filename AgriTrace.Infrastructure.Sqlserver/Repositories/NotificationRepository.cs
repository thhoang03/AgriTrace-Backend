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

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> AddAsync(
        Notification entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(ToModel(entity), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model == null)
            return;

        _context.Notifications.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Notifications
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<Notification?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<PagedResult<Notification>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Notification>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task UpdateAsync(
        Notification entity,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (model == null)
            return;

        model.Title = entity.Title;
        model.Message = entity.Message;
        model.IsRead = entity.IsRead;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    private static Notification ToEntity(NotificationDataModel model)
    {
        return Notification.Rehydrate(
            model.Id,
            model.UserId,
            model.Title,
            model.Message,
            model.IsRead,
            model.CreatedAt,
            model.UpdatedAt);
    }

    private static NotificationDataModel ToModel(Notification entity)
    {
        return new NotificationDataModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Message = entity.Message,
            IsRead = entity.IsRead,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

