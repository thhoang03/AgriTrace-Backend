using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Write;

public sealed class SupplyChainEventWriteRepository : ISupplyChainEventWriteRepository
{
    private readonly ApplicationDbContext _context;

    public SupplyChainEventWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.SupplyChainEvents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<SupplyChainEvent?> GetLastEventByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.SupplyChainEvents
            .Where(x => x.BatchId == batchId)
            .OrderByDescending(x => x.EventTime)
            .FirstOrDefaultAsync(cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<SupplyChainEvent> AddAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await _context.SupplyChainEvents.AddAsync(model, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        SupplyChainEvent entity,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.SupplyChainEvents
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (model == null)
            return;

        model.EventData = entity.EventData;
        model.Location = entity.Location;
        model.PreviousHash = entity.PreviousHash;
        model.CurrentHash = entity.CurrentHash;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.SupplyChainEvents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model == null)
            return;

        _context.SupplyChainEvents.Remove(model);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static SupplyChainEvent ToEntity(SupplyChainEventDataModel model) =>
        new(model.BatchId,
            model.EventTypeId,
            model.OrganizationId,
            model.PerformedByUserId,
            model.EventData,
            model.Location,
            model.PreviousHash,
            model.CurrentHash);

    private static SupplyChainEventDataModel ToModel(SupplyChainEvent entity) =>
        new()
        {
            Id = entity.Id,
            BatchId = entity.BatchId,
            EventTypeId = entity.EventTypeId,
            OrganizationId = entity.OrganizationId,
            PerformedByUserId = entity.PerformedByUserId,
            EventData = entity.EventData,
            Location = entity.Location,
            PreviousHash = entity.PreviousHash,
            CurrentHash = entity.CurrentHash,
            EventTime = entity.EventTime,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
}
