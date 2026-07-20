using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Notifications.Commands;

public record MarkAllNotificationsReadCommand(
    Guid UserId) : IRequest<MediatR.Unit>;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, MediatR.Unit>
{
    private readonly INotificationService _notificationService;

    public MarkAllNotificationsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<MediatR.Unit> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        // With auth wired (Phase 10) this is scoped to the user; an empty user id marks all as read.
        var notifications = request.UserId == Guid.Empty
            ? await _notificationService.GetAllAsync(cancellationToken)
            : await _notificationService.GetUnreadAsync(request.UserId, cancellationToken);

        foreach (var notification in notifications.Where(n => !n.IsRead))
        {
            notification.MarkAsRead();
            await _notificationService.UpdateAsync(notification, cancellationToken);
        }

        return MediatR.Unit.Value;
    }
}
