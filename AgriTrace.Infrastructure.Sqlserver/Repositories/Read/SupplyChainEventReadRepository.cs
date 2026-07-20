using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Read;

public sealed class SupplyChainEventReadRepository : ISupplyChainEventReadRepository
{
    private readonly ApplicationDbContext _context;

    public SupplyChainEventReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.SupplyChainEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<IReadOnlyList<SupplyChainEvent>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.SupplyChainEvents
            .AsNoTracking()
            .OrderByDescending(x => x.EventTime)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.SupplyChainEvents
            .AsNoTracking()
            .Where(x => x.BatchId == batchId)
            .OrderBy(x => x.EventTime)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<SupplyChainEvent>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.SupplyChainEvents
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.EventTime)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.SupplyChainEvents
            .AsNoTracking()
            .Where(x => x.EventTypeId == eventTypeId)
            .OrderBy(x => x.EventTime)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<PagedResult<SupplyChainEvent>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SupplyChainEvents.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.EventTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SupplyChainEvent>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
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
}
