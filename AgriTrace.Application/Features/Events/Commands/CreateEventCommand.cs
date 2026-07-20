using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Events.Commands;

/// <summary>
/// Result of creating an event. Mirrors swagger <c>EventCreatedData</c>.
/// </summary>
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
    private readonly IUserService _userService;

    public CreateEventCommandHandler(
        IEventService eventService,
        IBatchReadService batchReadService,
        IUserService userService)
    {
        _eventService = eventService;
        _batchReadService = batchReadService;
        _userService = userService;
    }

    public async Task<EventCreatedResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        // Organization is taken from the batch's current owner.
        var organizationId = batch.CurrentOrganizationId;

        // PerformedByUserId comes from the auth context in a later phase. Until then the controller
        // may pass Guid.Empty; the SupplyChainEvent entity requires a non-empty performer, so we
        // fall back to any existing user to keep the record valid (documented Phase 10 follow-up).
        var performedByUserId = request.PerformedByUserId;
        if (performedByUserId == Guid.Empty)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            var fallback = users.FirstOrDefault()
                ?? throw new ArgumentException(
                    "Cannot create event without a performing user (no users exist and auth is not yet wired).");

            performedByUserId = fallback.Id;
        }

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

        return new EventCreatedResult
        {
            EventId = created.Id,
            PreviousHash = created.PreviousHash,
            CurrentHash = created.CurrentHash
        };
    }
}
