using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record CreateEventRequestCommand(
    Guid EventTypeId,
    string? Location,
    string? Description) : IRequest<EventRequestDto>;

public class CreateEventRequestCommandHandler : IRequestHandler<CreateEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEventTypeRepository _eventTypeRepository;

    public CreateEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        IUserService userService,
        ICurrentUserService currentUser,
        IOrganizationRepository organizationRepository,
        IEventTypeRepository eventTypeRepository)
    {
        _eventRequestRepository = eventRequestRepository;
        _userService = userService;
        _currentUser = currentUser;
        _organizationRepository = organizationRepository;
        _eventTypeRepository = eventTypeRepository;
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

        // 3. Verify OrganizationId exists in DB
        Guid organizationId = user.OrganizationId ?? Guid.Empty;
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

        // 4. Check for duplicate pending/approved request for the same org + event type
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

        var entity = new EventRequest(
            batchId: null,
            request.EventTypeId,
            organizationId,
            _currentUser.UserId,
            eventData: null,
            request.Location,
            request.Description
        );

        await _eventRequestRepository.AddAsync(entity, cancellationToken);

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
