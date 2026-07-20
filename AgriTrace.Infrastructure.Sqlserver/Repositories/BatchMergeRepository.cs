using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories;

public class BatchMergeRepository : IBatchMergeRepository
{
    private readonly ApplicationDbContext _context;

    public BatchMergeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BatchMerge> AddAsync(
        BatchMerge entity,
        CancellationToken cancellationToken = default)
    {
        var model = new BatchMergeDataModel
        {
            Id = entity.Id,
            NewBatchId = entity.NewBatchId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Sources = entity.Sources.Select(s => new BatchMergeSourceDataModel
            {
                BatchMergeId = entity.Id,
                SourceBatchId = s.SourceBatchId,
                Quantity = s.Quantity
            }).ToList()
        };

        await _context.BatchMerges.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.BatchMerges
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model == null)
            return;

        _context.BatchMerges.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BatchMerge>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.BatchMerges
            .Include(x => x.Sources)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<BatchMerge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.BatchMerges
            .Include(x => x.Sources)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<PagedResult<BatchMerge>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.BatchMerges.Include(x => x.Sources).AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BatchMerge>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public Task UpdateAsync(
        BatchMerge entity,
        CancellationToken cancellationToken = default)
    {
        // Merge records are immutable audit entries; no update path required.
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<BatchMerge>> GetByNewBatchAsync(
        Guid newBatchId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.BatchMerges
            .Include(x => x.Sources)
            .Where(x => x.NewBatchId == newBatchId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    private static BatchMerge ToEntity(BatchMergeDataModel model)
    {
        var merge = new BatchMerge(model.NewBatchId);

        foreach (var source in model.Sources)
        {
            merge.AddSource(source.SourceBatchId, source.Quantity);
        }

        return merge;
    }
}
