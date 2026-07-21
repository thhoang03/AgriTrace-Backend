using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Write;

public sealed class ProductWriteRepository : IProductWriteRepository
{
    private readonly ApplicationDbContext _context;

    public ProductWriteRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Name == name,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<Product> AddAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        _context.Products.Add(model);

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);

        if (model == null)
        {
            return;
        }

        model.CategoryId = entity.CategoryId;
        model.UnitId = entity.UnitId;
        model.Name = entity.Name;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (model == null)
        {
            return;
        }

        _context.Products.Remove(model);

        await _context.SaveChangesAsync(cancellationToken);
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
            null,
            null);
    }

    private static ProductDataModel ToModel(Product entity)
    {
        return new ProductDataModel
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            CategoryId = entity.CategoryId,
            UnitId = entity.UnitId,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}