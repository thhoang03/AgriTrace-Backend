using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IReadRepository<TEntity, TKey>
{
    Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}