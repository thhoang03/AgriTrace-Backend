using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Entities.Products;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IProductWriteService
{
    Task<Product> CreateAsync(
        NewProduct request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateProduct request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}