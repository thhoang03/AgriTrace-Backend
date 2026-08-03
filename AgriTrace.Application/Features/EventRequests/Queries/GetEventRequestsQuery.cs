using AgriTrace.Domain.Common;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.EventRequests.Commands;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Queries;

public record GetEventRequestsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? BatchId = null,
    EventRequestStatus? Status = null,
    bool OnlyMine = false) : IRequest<PagedResult<EventRequestDto>>;

public class GetEventRequestsQueryHandler : IRequestHandler<GetEventRequestsQuery, PagedResult<EventRequestDto>>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEventRequestsQueryHandler(
        IEventRequestRepository eventRequestRepository,
        ICurrentUserService currentUser)
    {
        _eventRequestRepository = eventRequestRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<EventRequestDto>> Handle(GetEventRequestsQuery request, CancellationToken cancellationToken)
    {
        Guid? myUserId = request.OnlyMine && _currentUser.IsAuthenticated ? _currentUser.UserId : null;

        var paged = await _eventRequestRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.BatchId,
            request.Status,
            organizationId: null,
            requestedByUserId: myUserId,
            cancellationToken: cancellationToken
        );

        var dtos = paged.Items.Select(CreateEventRequestCommandHandler.MapToDto).ToList();

        return new PagedResult<EventRequestDto>(
            dtos,
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize
        );
    }
}
