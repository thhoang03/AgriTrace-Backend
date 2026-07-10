using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IUserRepository
    : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetByRoleAsync(
        string role,
        CancellationToken cancellationToken = default);
}