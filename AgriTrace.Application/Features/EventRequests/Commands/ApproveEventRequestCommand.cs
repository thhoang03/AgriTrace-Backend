using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Events.Commands;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record ApproveEventRequestCommand(Guid RequestId) : IRequest<EventRequestDto>;

public class ApproveEventRequestCommandHandler : IRequestHandler<ApproveEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ApproveEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        ISender sender,
        ICurrentUserService currentUser)
    {
        _eventRequestRepository = eventRequestRepository;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<EventRequestDto> Handle(ApproveEventRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new ForbiddenException("User is not authenticated");

        var eventReq = await _eventRequestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException($"EventRequest {request.RequestId} not found");

        eventReq.Approve(_currentUser.UserId);

        // Create actual SupplyChainEvent & Compute Hash Chain
        await _sender.Send(new CreateEventCommand(
            eventReq.BatchId,
            eventReq.EventTypeId,
            eventReq.EventData,
            eventReq.Location,
            eventReq.RequestedByUserId
        ), cancellationToken);

        await _eventRequestRepository.UpdateAsync(eventReq, cancellationToken);

        var updated = await _eventRequestRepository.GetByIdAsync(eventReq.Id, cancellationToken) ?? eventReq;
        return CreateEventRequestCommandHandler.MapToDto(updated);
    }
}
