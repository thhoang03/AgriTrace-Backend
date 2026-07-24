using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface ICategoryRepository
    : IRepository<Category, Guid>
{
    Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Category>> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<bool> HasProductsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}