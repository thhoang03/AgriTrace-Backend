using AgriTrace.Domain.Interfaces.Outbound;

public interface IRepository<TEntity, TKey>
    : IReadRepository<TEntity, TKey>,
      IWriteRepository<TEntity, TKey>
{
}