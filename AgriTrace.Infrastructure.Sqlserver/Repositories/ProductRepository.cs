using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class ProductRepository
    : IProductRepository
{

    private readonly ApplicationDbContext _context;



    public ProductRepository(
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





    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Products
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    public async Task<Product> AddAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {

        var model = ToModel(entity);



        await _context.Products
            .AddAsync(
                model,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



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
            return;



        model.CategoryId =
            entity.CategoryId;


        model.UnitId =
            entity.UnitId;


        model.Name =
            entity.Name;



        model.UpdatedAt =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);

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
            return;



        _context.Products.Remove(model);



        await _context.SaveChangesAsync(
            cancellationToken);

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





    public async Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Products
            .Where(
                x => x.OrganizationId == organizationId)
            .OrderBy(
                x => x.Name)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Products
            .Where(
                x => x.CategoryId == categoryId)
            .OrderBy(
                x => x.Name)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    public async Task<PagedResult<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {

        var query = _context.Products
            .AsQueryable();



        var totalCount =
            await query.CountAsync(cancellationToken);



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





    private static Product ToEntity(
        ProductDataModel model)
    {

        return new Product(
            model.OrganizationId,
            model.CategoryId,
            model.UnitId,
            model.Name);

    }





    private static ProductDataModel ToModel(
        Product entity)
    {

        return new ProductDataModel
        {

            Id = entity.Id,


            OrganizationId =
                entity.OrganizationId,


            CategoryId =
                entity.CategoryId,


            UnitId =
                entity.UnitId,


            Name =
                entity.Name,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}