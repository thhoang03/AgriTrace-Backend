using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IReadService<TEntity, TKey>
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