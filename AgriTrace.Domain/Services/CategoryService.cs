using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class CategoryService : ICategoryService
{

    private readonly ICategoryRepository _repository;



    public CategoryService(
        ICategoryRepository repository)
    {
        _repository = repository;
    }





    public async Task<Category> CreateAsync(
        Category entity,
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





    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }





    public async Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }





    public async Task<PagedResult<Category>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(null, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedResult<Category>> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            search,
            pageNumber,
            pageSize,
            cancellationToken);
    }





    public async Task UpdateAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }





    public async Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByNameAsync(
            name,
            cancellationToken);
    }

    public async Task<bool> HasProductsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.HasProductsAsync(
            categoryId,
            cancellationToken);
    }
}