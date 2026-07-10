using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class ProductService : IProductService
{

    private readonly IProductRepository _repository;


    public ProductService(
        IProductRepository repository)
    {
        _repository = repository;
    }



    public async Task<Product> CreateAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(
            entity,
            cancellationToken);
    }



    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(
            id,
            cancellationToken);
    }




    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }




    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }




    public async Task<PagedResult<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }




    public async Task UpdateAsync(
        Product entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }





    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByCategoryAsync(
            categoryId,
            cancellationToken);
    }





    public async Task<IReadOnlyList<Product>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);
    }





    public async Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByNameAsync(
            name,
            cancellationToken);
    }

}