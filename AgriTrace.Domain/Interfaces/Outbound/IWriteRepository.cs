namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IWriteRepository<TEntity, TKey>
{
    Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        TKey id,
        CancellationToken cancellationToken = default);
}