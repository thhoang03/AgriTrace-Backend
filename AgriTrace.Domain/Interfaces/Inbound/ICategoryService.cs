using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ICategoryService
    : IService<Category, Guid>
{
    Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}