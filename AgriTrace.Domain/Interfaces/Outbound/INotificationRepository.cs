using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface INotificationRepository
    : IRepository<Notification, Guid>
{
    Task<IReadOnlyList<Notification>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetUnreadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}