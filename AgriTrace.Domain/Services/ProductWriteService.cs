using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public sealed class ProductWriteService : IProductWriteService
{
    private readonly IProductWriteRepository _repository;

    public ProductWriteService(
        IProductWriteRepository repository)
    {
        _repository = repository;
    }

    public Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => _repository.GetByNameAsync(name, cancellationToken);

    public Task<Product> CreateAsync(
        NewProduct request,
        CancellationToken cancellationToken = default)
    {
        var product = new Product(
            request.OrganizationId,
            request.CategoryId,
            request.UnitId,
            request.Name);

        return _repository.AddAsync(product, cancellationToken);
    }

    public async Task UpdateAsync(
       Guid id,
       UpdateProduct request,
       CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        product.UpdateInformation(
            request.CategoryId,
            request.UnitId,
            request.Name,
            request.OrganizationId);

        await _repository.UpdateAsync(
            product,
            cancellationToken);
    }

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
