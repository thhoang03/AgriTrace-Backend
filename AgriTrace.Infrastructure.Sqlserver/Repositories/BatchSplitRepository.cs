using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories;

public class BatchSplitRepository : IBatchSplitRepository
{
    private readonly ApplicationDbContext _context;

    public BatchSplitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BatchSplit> AddAsync(
        BatchSplit entity,
        CancellationToken cancellationToken = default)
    {
        var model = new BatchSplitDataModel
        {
            Id = entity.Id,
            SourceBatchId = entity.SourceBatchId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Details = entity.Details.Select(d => new BatchSplitDetailDataModel
            {
                Id = d.Id,
                SplitId = entity.Id,
                TargetBatchId = d.TargetBatchId,
                Quantity = d.Quantity,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList()
        };

        await _context.BatchSplits.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.BatchSplits
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model == null)
            return;

        _context.BatchSplits.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BatchSplit>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.BatchSplits
            .Include(x => x.Details)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<BatchSplit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.BatchSplits
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<PagedResult<BatchSplit>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.BatchSplits.Include(x => x.Details).AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BatchSplit>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public Task UpdateAsync(
        BatchSplit entity,
        CancellationToken cancellationToken = default)
    {
        // Split records are immutable audit entries; no update path required.
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<BatchSplit>> GetBySourceBatchAsync(
        Guid sourceBatchId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.BatchSplits
            .Include(x => x.Details)
            .Where(x => x.SourceBatchId == sourceBatchId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    private static BatchSplit ToEntity(BatchSplitDataModel model)
    {
        var split = new BatchSplit(model.SourceBatchId);

        foreach (var detail in model.Details)
        {
            split.AddDetail(detail.TargetBatchId, detail.Quantity);
        }

        return split;
    }

    public Task SaveSplitAsync(Batch sourceBatch, IReadOnlyList<Batch> childBatches, BatchSplit batchSplit, IReadOnlyList<BatchSplitDetail> details, SupplyChainEvent splitEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
