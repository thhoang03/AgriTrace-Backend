using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Notifications.Commands;

public record MarkNotificationReadCommand(
    Guid NotificationId,
    Guid UserId) : IRequest<MediatR.Unit>;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, MediatR.Unit>
{
    private readonly INotificationService _notificationService;

    public MarkNotificationReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<MediatR.Unit> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetByIdAsync(request.NotificationId, cancellationToken)
            ?? throw new NotFoundException($"Notification {request.NotificationId} not found.");

        notification.MarkAsRead();
        await _notificationService.UpdateAsync(notification, cancellationToken);

        return MediatR.Unit.Value;
    }
}
