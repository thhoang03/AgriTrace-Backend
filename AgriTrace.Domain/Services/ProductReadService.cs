using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
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