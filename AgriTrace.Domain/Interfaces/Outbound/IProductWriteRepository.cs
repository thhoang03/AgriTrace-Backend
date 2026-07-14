using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IProductWriteRepository
{
    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<Product> AddAsync(
        Product entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Product entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}