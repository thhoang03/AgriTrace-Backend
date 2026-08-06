using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record ApproveEventRequestCommand(Guid RequestId) : IRequest<EventRequestDto>;

public class ApproveEventRequestCommandHandler : IRequestHandler<ApproveEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService? _notificationService;
    private readonly IUserService? _userService;

    public ApproveEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        ICurrentUserService currentUser,
        INotificationService? notificationService = null,
        IUserService? userService = null)
    {
        _eventRequestRepository = eventRequestRepository;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _userService = userService;
    }

    public async Task<EventRequestDto> Handle(ApproveEventRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new ForbiddenException("User is not authenticated");

        if (_currentUser.Role != "Admin")
            throw new ForbiddenException("Chỉ Admin hệ thống mới có quyền phê duyệt yêu cầu mở rộng event type.");

        var eventReq = await _eventRequestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException($"EventRequest {request.RequestId} not found");

        // Grant org-level permission — mark as Approved only, do NOT create any SupplyChainEvent
        eventReq.Approve(_currentUser.UserId);

        await _eventRequestRepository.UpdateAsync(eventReq, cancellationToken);

        if (_notificationService != null && _userService != null)
        {
            var orgUsers = await _userService.GetByOrganizationAsync(eventReq.OrganizationId, cancellationToken);
            foreach (var u in orgUsers)
            {
                var eventName = eventReq.EventType?.Name ?? "sự kiện";
                var notif = new Notification(
                    u.Id,
                    "✅ YÊU CẦU ĐÃ ĐƯỢC PHÊ DUYỆT",
                    $"Yêu cầu mở rộng quy trình cho sự kiện '{eventName}' của tổ chức bạn đã được Quản trị viên hệ thống phê duyệt.");
                await _notificationService.CreateAsync(notif, cancellationToken);
            }
        }

        var updated = await _eventRequestRepository.GetByIdAsync(eventReq.Id, cancellationToken) ?? eventReq;
        return CreateEventRequestCommandHandler.MapToDto(updated);
    }
}
