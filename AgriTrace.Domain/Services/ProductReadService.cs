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
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public sealed class ProductReadService : IProductReadService
{
    private readonly IProductReadRepository _repository;

    public ProductReadService(
        IProductReadRepository repository)
    {
        _repository = repository;
    }

    public Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
        => _repository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);

    public Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
        => _repository.GetByCategoryAsync(
            categoryId,
            cancellationToken);

    public Task<PagedResult<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<PagedResult<Product>> SearchAsync(
        Guid? organizationId,
        Guid? categoryId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => _repository.SearchAsync(
            organizationId,
            categoryId,
            search,
            pageNumber,
            pageSize,
            cancellationToken);
}
