using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Read;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly ApplicationDbContext _context;

    public ProductReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<PagedResult<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
       .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit);

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<PagedResult<Product>> SearchAsync(
        Guid? organizationId,
        Guid? categoryId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductDataModel> query = _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Unit);

        if (organizationId.HasValue)
        {
            query = query.Where(x => x.OrganizationId == organizationId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x => x.Name.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    private static Product ToEntity(ProductDataModel model)
    {
        return new Product(
            model.Id,
            model.OrganizationId,
            model.CategoryId,
            model.UnitId,
            model.Name,
            model.CreatedAt,
            model.UpdatedAt,
            model.Category == null ? null : new Category(
                model.Category.Id,
                model.Category.Name,
                model.Category.Description,
                model.Category.CreatedAt,
                model.Category.UpdatedAt,
                model.Category.IsActive),
            model.Unit == null ? null : new Unit(
                model.Unit.Id,
                model.Unit.Code,
                model.Unit.Name,
                model.Unit.CreatedAt,
                model.Unit.UpdatedAt));
    }
}