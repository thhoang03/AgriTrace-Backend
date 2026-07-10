namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IWriteService<TEntity, TKey>
{
    Task<TEntity> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        TKey id,
        CancellationToken cancellationToken = default);
}