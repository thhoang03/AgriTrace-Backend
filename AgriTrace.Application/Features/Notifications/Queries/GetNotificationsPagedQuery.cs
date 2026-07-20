using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Notifications.Queries;

public record GetNotificationsPagedQuery(
    Guid UserId,
    bool? IsRead,
    int Page,
    int PageSize) : IRequest<PagedResult<NotificationDto>>;

public class GetNotificationsPagedQueryHandler : IRequestHandler<GetNotificationsPagedQuery, PagedResult<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public GetNotificationsPagedQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsPagedQuery request, CancellationToken cancellationToken)
    {
        // With auth wired (Phase 10) notifications are scoped to the user. Until then, an empty user id
        // returns all notifications so the endpoint is usable.
        var source = request.UserId == Guid.Empty
            ? await _notificationService.GetAllAsync(cancellationToken)
            : await _notificationService.GetByUserAsync(request.UserId, cancellationToken);

        var filtered = request.IsRead.HasValue
            ? source.Where(n => n.IsRead == request.IsRead.Value)
            : source;

        var all = filtered.ToList();
        var totalCount = all.Count;

        var items = all
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(NotificationMapper.ToDto)
            .ToList();

        return new PagedResult<NotificationDto>(items, totalCount, request.Page, request.PageSize);
    }
}
