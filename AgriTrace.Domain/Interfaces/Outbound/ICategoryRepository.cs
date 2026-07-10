using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface ICategoryRepository
    : IRepository<Category, Guid>
{
    Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}