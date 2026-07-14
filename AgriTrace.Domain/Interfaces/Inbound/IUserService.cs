using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IUserService
    : IService<User, Guid>
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default);
}