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
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Read;

public sealed class BatchReadRepository
    : IBatchReadRepository
{
    private readonly ApplicationDbContext _context;

    public BatchReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<BatchDataModel> QueryWithIncludes()
    {
        return _context.Batches
            .AsNoTracking()
            .Include(x => x.Product)
                .ThenInclude(p => p.Category)
            .Include(x => x.Product)
                .ThenInclude(p => p.Organization)
            .Include(x => x.Unit)
            .Include(x => x.CurrentOrganization);
    }

    public async Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await QueryWithIncludes()
            .Include(x => x.ParentBatch)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await QueryWithIncludes()
            .OrderBy(x => x.BatchCode)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var models = await QueryWithIncludes()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.BatchCode)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var models = await QueryWithIncludes()
            .Where(x => x.CurrentOrganizationId == organizationId)
            .OrderBy(x => x.BatchCode)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default)
    {
        var model = await QueryWithIncludes()
            .FirstOrDefaultAsync(
                x => x.BatchCode == batchCode,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default)
    {
        var models = await QueryWithIncludes()
            .Where(x => x.ParentBatchId == parentBatchId)
            .OrderBy(x => x.BatchCode)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default)
    {
        var models = await QueryWithIncludes()
            .Where(x => x.RootBatchId == rootBatchId)
            .OrderBy(x => x.BatchCode)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = QueryWithIncludes();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderBy(x => x.BatchCode)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Batch>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<PagedResult<Batch>> SearchAsync(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BatchDataModel> query = QueryWithIncludes();

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (organizationId.HasValue)
        {
            query = query.Where(x => x.CurrentOrganizationId == organizationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.BatchCode.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderBy(x => x.BatchCode)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Batch>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    private static Batch ToEntity(BatchDataModel model)
    {
        Product? product = null;
        if (model.Product != null)
        {
            Category? category = model.Product.Category == null ? null : new Category(
                model.Product.Category.Id,
                model.Product.Category.Name,
                model.Product.Category.Description,
                model.Product.Category.CreatedAt,
                model.Product.Category.UpdatedAt,
                model.Product.Category.IsActive);

            Organization? org = model.Product.Organization == null ? null : new Organization(
                model.Product.Organization.Id,
                model.Product.Organization.OrganizationTypeId,
                model.Product.Organization.Name,
                model.Product.Organization.Address,
                model.Product.Organization.Status,
                model.Product.Organization.CreatedAt,
                model.Product.Organization.UpdatedAt,
                null);

            product = new Product(
                model.Product.Id,
                model.Product.OrganizationId,
                model.Product.CategoryId,
                model.Product.UnitId,
                model.Product.Name,
                model.Product.CreatedAt,
                model.Product.UpdatedAt,
                category,
                null,
                org);
        }

        Unit? unit = model.Unit == null ? null : new Unit(
            model.Unit.Id,
            model.Unit.Code,
            model.Unit.Name,
            model.Unit.CreatedAt,
            model.Unit.UpdatedAt);

        Organization? currentOrg = model.CurrentOrganization == null ? null : new Organization(
            model.CurrentOrganization.Id,
            model.CurrentOrganization.OrganizationTypeId,
            model.CurrentOrganization.Name,
            model.CurrentOrganization.Address,
            model.CurrentOrganization.Status,
            model.CurrentOrganization.CreatedAt,
            model.CurrentOrganization.UpdatedAt,
            null);

        return Batch.Rehydrate(
            id:                   model.Id,
            productId:            model.ProductId,
            batchCode:            model.BatchCode,
            quantity:             model.Quantity,
            remainingQuantity:    model.RemainingQuantity,
            sourceQuantity:       model.SourceQuantity,
            unitId:               model.UnitId,
            productionDate:       model.ProductionDate,
            expiryDate:           model.ExpiryDate,
            status:               model.Status,
            currentOrganizationId: model.CurrentOrganizationId,
            qrCode:               model.QRCode,
            parentBatchId:        model.ParentBatchId,
            rootBatchId:          model.RootBatchId,
            splitId:              model.SplitId,
            createdAt:            model.CreatedAt,
            updatedAt:            model.UpdatedAt,
            product:              product,
            unit:                 unit,
            currentOrganization:  currentOrg);
    }
}
