using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record CreateEventRequestCommand(
    Guid EventTypeId,
    Guid? BatchId,
    Guid? TargetOrganizationId,
    string? Location,
    string? Description) : IRequest<EventRequestDto>;

public class CreateEventRequestCommandHandler : IRequestHandler<CreateEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly INotificationService? _notificationService;
    private readonly IQualityInspectionService? _qualityInspectionService;

    public CreateEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        IUserService userService,
        ICurrentUserService currentUser,
        IOrganizationRepository organizationRepository,
        IEventTypeRepository eventTypeRepository,
        INotificationService? notificationService = null,
        IQualityInspectionService? qualityInspectionService = null)
    {
        _eventRequestRepository = eventRequestRepository;
        _userService = userService;
        _currentUser = currentUser;
        _organizationRepository = organizationRepository;
        _eventTypeRepository = eventTypeRepository;
        _notificationService = notificationService;
        _qualityInspectionService = qualityInspectionService;
    }

    public async Task<EventRequestDto> Handle(CreateEventRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new ForbiddenException("User is not authenticated");

        var user = await _userService.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {_currentUser.UserId} not found");

        // 1. Verify EventTypeId exists in DB
        if (request.EventTypeId == Guid.Empty)
            throw new ConflictException("EventType ID is required to submit an expansion request.");

        var targetEventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, cancellationToken)
            ?? throw new NotFoundException($"EventType {request.EventTypeId} not found.");

        // 2. Guard: SPLIT, MERGE, RECALL cannot be requested as expansions
        var targetCode = targetEventType.Code?.ToUpper() ?? "";
        if (targetCode is "SPLIT" or "MERGE" or "RECALL")
            throw new ConflictException($"Event type '{targetCode}' is a system-level operation and cannot be requested as an organization expansion.");

        // 3. Determine Target Organization
        Guid organizationId = request.TargetOrganizationId ?? user.OrganizationId ?? Guid.Empty;
        if (organizationId != Guid.Empty)
        {
            var org = await _organizationRepository.GetByIdAsync(organizationId, cancellationToken);
            if (org == null)
                organizationId = Guid.Empty;
        }
        if (organizationId == Guid.Empty)
        {
            var allOrgs = await _organizationRepository.GetAllAsync(cancellationToken);
            organizationId = allOrgs.FirstOrDefault()?.Id
                ?? throw new ConflictException("No Organization exists in the system.");
        }

        // 4. Check for duplicate pending/approved request (only if it's NOT a targeted inspection request)
        if (request.TargetOrganizationId == null || request.TargetOrganizationId == Guid.Empty)
        {
            var existing = await _eventRequestRepository.GetAllAsync(cancellationToken);
            var duplicate = existing.FirstOrDefault(r =>
                r.OrganizationId == organizationId &&
                r.EventTypeId == request.EventTypeId &&
                (r.Status == Domain.Enums.EventRequestStatus.Pending || r.Status == Domain.Enums.EventRequestStatus.Approved));
            if (duplicate != null)
            {
                var statusLabel = duplicate.Status == Domain.Enums.EventRequestStatus.Approved ? "đã được phê duyệt" : "đang chờ xét duyệt";
                throw new ConflictException($"Tổ chức của bạn đã có yêu cầu mở rộng cho event type '{targetEventType.Name}' ({statusLabel}).");
            }
        }

        var entity = new EventRequest(
            batchId: request.BatchId,
            request.EventTypeId,
            organizationId,
            _currentUser.UserId,
            eventData: null,
            request.Location,
            request.Description
        );

        await _eventRequestRepository.AddAsync(entity, cancellationToken);



        if (_notificationService != null)
        {
            var orgName = "Tổ chức không xác định";
            var myOrgId = user.OrganizationId ?? Guid.Empty;
            if (myOrgId != Guid.Empty)
            {
                var myOrg = await _organizationRepository.GetByIdAsync(myOrgId, cancellationToken);
                if (myOrg != null) orgName = myOrg.Name;
            }

            if (request.TargetOrganizationId.HasValue && request.TargetOrganizationId != Guid.Empty)
            {
                // Notify the Target Organization
                var targetUsers = await _userService.GetByOrganizationAsync(request.TargetOrganizationId.Value, cancellationToken);
                foreach (var u in targetUsers)
                {
                    var titleJson = System.Text.Json.JsonSerializer.Serialize(new { en = "📝 NEW INSPECTION REQUEST", vi = "📝 CÓ YÊU CẦU KIỂM ĐỊNH MỚI" });
                    var msgJson = System.Text.Json.JsonSerializer.Serialize(new { 
                        en = $"Organization '{orgName}' has sent a new inspection request for their batch. Please review and process.",
                        vi = $"Tổ chức '{orgName}' vừa gửi một yêu cầu kiểm định cho lô hàng của họ. Vui lòng kiểm tra và xử lý."
                    });

                    var notif = new Notification(
                        u.Id,
                        titleJson,
                        msgJson,
                        "INSPECTION");
                    await _notificationService.CreateAsync(notif, cancellationToken);
                }
            }
            else
            {
                // Notify System Admins
                var systemAdmins = await _userService.GetByRoleAsync(Domain.Entities.Users.UserRole.Admin, cancellationToken);
                foreach (var admin in systemAdmins)
                {
                    var titleJson = System.Text.Json.JsonSerializer.Serialize(new { en = "📝 NEW EXPANSION REQUEST", vi = "📝 CÓ YÊU CẦU MỞ RỘNG MỚI" });
                    var msgJson = System.Text.Json.JsonSerializer.Serialize(new { 
                        en = $"Organization '{orgName}' has sent a new process expansion request for event type '{targetEventType.Name}'. Please review and approve.",
                        vi = $"Tổ chức '{orgName}' vừa gửi một yêu cầu mở rộng quy trình mới cho sự kiện '{targetEventType.Name}'. Vui lòng kiểm tra và xét duyệt."
                    });

                    var notif = new Notification(
                        admin.Id,
                        titleJson,
                        msgJson,
                        "SYSTEM");
                    await _notificationService.CreateAsync(notif, cancellationToken);
                }
            }
        }

        var created = await _eventRequestRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToDto(created);
    }

    public static EventRequestDto MapToDto(EventRequest req)
    {
        return new EventRequestDto
        {
            Id = req.Id,
            BatchId = req.BatchId ?? Guid.Empty,
            BatchCode = req.Batch?.BatchCode,
            EventTypeId = req.EventTypeId,
            EventTypeCode = req.EventType?.Code,
            EventTypeName = req.EventType?.Name,
            OrganizationId = req.OrganizationId,
            OrganizationName = req.Organization?.Name,
            RequestedByUserId = req.RequestedByUserId,
            RequestedByUserName = req.RequestedByUser?.FullName,
            EventData = req.EventData,
            Location = req.Location,
            Description = req.Description,
            Status = req.Status,
            RejectionReason = req.RejectionReason,
            CreatedAt = req.CreatedAt,
            ReviewedAt = req.ReviewedAt,
            ReviewedByUserId = req.ReviewedByUserId
        };
    }
}
