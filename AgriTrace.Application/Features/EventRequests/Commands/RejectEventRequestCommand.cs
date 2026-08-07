using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record RejectEventRequestCommand(Guid RequestId, string? Reason) : IRequest<EventRequestDto>;

public class RejectEventRequestCommandHandler : IRequestHandler<RejectEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService? _notificationService;
    private readonly IUserService? _userService;

    public RejectEventRequestCommandHandler(
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

    public async Task<EventRequestDto> Handle(RejectEventRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new ForbiddenException("User is not authenticated");

        if (_currentUser.Role != "Admin" && _currentUser.Role != "Manager")
            throw new ForbiddenException("Chỉ Quản lý (Manager) hoặc Admin mới có quyền từ chối yêu cầu sự kiện.");

        var eventReq = await _eventRequestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException($"EventRequest {request.RequestId} not found");

        eventReq.Reject(_currentUser.UserId, request.Reason);

        await _eventRequestRepository.UpdateAsync(eventReq, cancellationToken);

        if (_notificationService != null && _userService != null)
        {
            var orgUsers = await _userService.GetByOrganizationAsync(eventReq.OrganizationId, cancellationToken);
            var reason = string.IsNullOrEmpty(request.Reason) ? "Không có lý do được cung cấp" : request.Reason;
            foreach (var u in orgUsers)
            {
                var eventName = eventReq.EventType?.Name ?? "sự kiện";
                var titleJson = System.Text.Json.JsonSerializer.Serialize(new { en = "❌ REQUEST REJECTED", vi = "❌ YÊU CẦU BỊ TỪ CHỐI" });
                var reasonEn = string.IsNullOrEmpty(request.Reason) ? "No reason provided" : request.Reason;
                var msgJson = System.Text.Json.JsonSerializer.Serialize(new { 
                    en = $"Your organization's process expansion request for event '{eventName}' has been REJECTED. Reason: {reasonEn}.",
                    vi = $"Yêu cầu mở rộng quy trình cho sự kiện '{eventName}' của tổ chức bạn đã bị TỪ CHỐI. Lý do: {reason}."
                });

                var notif = new Notification(
                    u.Id,
                    titleJson,
                    msgJson);
                await _notificationService.CreateAsync(notif, cancellationToken);
            }
        }

        var updated = await _eventRequestRepository.GetByIdAsync(eventReq.Id, cancellationToken) ?? eventReq;
        return CreateEventRequestCommandHandler.MapToDto(updated);
    }
}
