namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IService<TEntity, TKey>
    : IReadService<TEntity, TKey>,
      IWriteService<TEntity, TKey>
{
}