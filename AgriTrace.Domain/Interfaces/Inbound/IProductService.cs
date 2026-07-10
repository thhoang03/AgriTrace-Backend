using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IProductService
    : IService<Product, Guid>
{

    Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);



    Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

}