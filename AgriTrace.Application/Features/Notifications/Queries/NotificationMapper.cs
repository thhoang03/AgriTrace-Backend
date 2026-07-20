using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Application.Features.Notifications.Queries;

public static class NotificationMapper
{
    public static NotificationDto ToDto(Notification n) => new()
    {
        NotificationId = n.Id,
        UserId = n.UserId,
        Title = n.Title,
        Message = n.Message,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
