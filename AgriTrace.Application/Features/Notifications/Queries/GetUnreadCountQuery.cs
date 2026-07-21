using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Notifications.Queries;

public record GetUnreadCountQuery(
    Guid UserId) : IRequest<int>;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly INotificationService _notificationService;

    public GetUnreadCountQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            var all = await _notificationService.GetAllAsync(cancellationToken);
            return all.Count(n => !n.IsRead);
        }

        var unread = await _notificationService.GetUnreadAsync(request.UserId, cancellationToken);
        return unread.Count;
    }
}
