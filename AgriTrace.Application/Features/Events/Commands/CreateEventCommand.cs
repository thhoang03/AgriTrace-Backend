using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Events.Commands;

public class EventCreatedResult
{
    public Guid EventId { get; set; }

    public string? PreviousHash { get; set; }

    public string? CurrentHash { get; set; }
}

public record CreateEventCommand(
    Guid BatchId,
    Guid EventTypeId,
    string? EventData,
    string? Location,
    Guid PerformedByUserId) : IRequest<EventCreatedResult>;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventCreatedResult>
{
    private readonly IEventService _eventService;
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IUserService _userService;
    private readonly IOrganizationService _organizationService;
    private readonly IEventTypeService _eventTypeService;
    private readonly ICurrentUserService _currentUser;

    public CreateEventCommandHandler(
        IEventService eventService,
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IUserService userService,
        IOrganizationService organizationService,
        IEventTypeService eventTypeService,
        ICurrentUserService currentUser)
    {
        _eventService = eventService;
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _userService = userService;
        _organizationService = organizationService;
        _eventTypeService = eventTypeService;
        _currentUser = currentUser;
    }

    public async Task<EventCreatedResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        var performedByUserId = request.PerformedByUserId;
        if (performedByUserId == Guid.Empty)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            var fallback = users.FirstOrDefault()
                ?? throw new ArgumentException(
                    "Cannot create event without a performing user (no users exist and auth is not yet wired).");

            performedByUserId = fallback.Id;
        }

        if (_currentUser.IsAuthenticated && performedByUserId != Guid.Empty && performedByUserId != _currentUser.UserId)
        {
            throw new RbacForbiddenException("RBAC_IDOR_ATTEMPT", "PerformedByUserId does not match authenticated user.");
        }

        var user = await _userService.GetByIdAsync(performedByUserId, cancellationToken)
            ?? throw new NotFoundException($"User {performedByUserId} not found.");

        var eventType = await _eventTypeService.GetByIdAsync(request.EventTypeId, cancellationToken)
            ?? throw new NotFoundException($"EventType {request.EventTypeId} not found.");

        var orgTypeCode = _currentUser.OrganizationType;

        bool isAdmin = user.Role == UserRole.Admin;
        bool isInspectionCrossOrg = orgTypeCode == "INSPECTION" && eventType.Code == "INSPECTION";

        if (!isAdmin && !isInspectionCrossOrg)
        {
            if (!AgriTrace.Domain.Common.EventPermissionRules.IsAllowed(orgTypeCode ?? string.Empty, eventType.Code))
            {
                throw new ForbiddenException($"Organization type '{orgTypeCode}' is not allowed to create event type '{eventType.Code}'.");
            }
        }

        bool isReceiveEvent = eventType.Code == "RECEIVE";

        if (!isInspectionCrossOrg && user.OrganizationId.HasValue)
        {
            if (isReceiveEvent)
            {
                if (batch.CurrentOrganizationId == user.OrganizationId.Value)
                {
                    throw new ForbiddenException($"Batch {request.BatchId} already belongs to your organization. Cannot receive a batch you already own.");
                }
            }
            else
            {
                if (batch.CurrentOrganizationId != user.OrganizationId.Value)
                {
                    throw new ForbiddenException($"Batch {request.BatchId} is not owned by the current user's organization.");
                }
            }
        }

        var organizationId = isReceiveEvent ? user.OrganizationId!.Value : batch.CurrentOrganizationId;

        var entity = new SupplyChainEvent(
            request.BatchId,
            request.EventTypeId,
            organizationId,
            performedByUserId,
            request.EventData,
            request.Location,
            previousHash: null,
            currentHash: null);

        var created = await _eventService.CreateEventAsync(entity, cancellationToken);

        if (isReceiveEvent && user.OrganizationId.HasValue)
        {
            batch.ChangeOrganization(user.OrganizationId.Value);
            await _batchWriteService.UpdateAsync(batch, cancellationToken);
        }

        return new EventCreatedResult
        {
            EventId = created.Id,
            PreviousHash = created.PreviousHash,
            CurrentHash = created.CurrentHash
        };
    }
}

