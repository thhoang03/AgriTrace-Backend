using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories;

public class CategoryRepository
    : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Categories
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<PagedResult<Category>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(null, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedResult<Category>> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var entities = models.Select(ToEntity).ToList();

        return new PagedResult<Category>(
            entities,
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<Category> AddAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await _context.Categories
            .AddAsync(
                model,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);

        if (model == null)
            return;

        model.Name = entity.Name;
        model.Description = entity.Description;
        model.IsActive = entity.IsActive;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (model == null)
            return;

        _context.Categories.Remove(model);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Name == name,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<bool> HasProductsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AnyAsync(
                x => x.CategoryId == categoryId,
                cancellationToken);
    }

    private static Category ToEntity(
        CategoryDataModel model)
    {
        return new Category(
            model.Id,
            model.Name,
            model.Description,
            model.CreatedAt,
            model.UpdatedAt,
            model.IsActive);
    }

    private static CategoryDataModel ToModel(
        Category entity)
    {
        return new CategoryDataModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
