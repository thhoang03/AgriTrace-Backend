using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IProductReadService
{
    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> SearchAsync(
        Guid? organizationId,
        Guid? categoryId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}