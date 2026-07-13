using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface INotificationService
    : IService<Notification, Guid>
{
    Task<IReadOnlyList<Notification>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetUnreadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}