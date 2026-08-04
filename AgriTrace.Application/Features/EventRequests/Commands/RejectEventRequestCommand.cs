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

    public RejectEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        ICurrentUserService currentUser)
    {
        _eventRequestRepository = eventRequestRepository;
        _currentUser = currentUser;
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

        var updated = await _eventRequestRepository.GetByIdAsync(eventReq.Id, cancellationToken) ?? eventReq;
        return CreateEventRequestCommandHandler.MapToDto(updated);
    }
}
