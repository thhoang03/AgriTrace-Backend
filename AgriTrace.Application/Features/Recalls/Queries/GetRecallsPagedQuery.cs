using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Recalls.Queries;

public record GetRecallsPagedQuery(
    int Page,
    int PageSize) : IRequest<PagedResult<RecallDto>>;

public class GetRecallsPagedQueryHandler : IRequestHandler<GetRecallsPagedQuery, PagedResult<RecallDto>>
{
    private readonly IRecallService _recallService;
    private readonly IBatchReadService _batchReadService;
    private readonly IUserService _userService;

    public GetRecallsPagedQueryHandler(
        IRecallService recallService,
        IBatchReadService batchReadService,
        IUserService userService)
    {
        _recallService = recallService;
        _batchReadService = batchReadService;
        _userService = userService;
    }

    public async Task<PagedResult<RecallDto>> Handle(GetRecallsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await _recallService.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        var items = new List<RecallDto>();
        foreach (var recall in paged.Items)
        {
            items.Add(await RecallMapper.ToDtoAsync(recall, _batchReadService, _userService, cancellationToken));
        }

        return new PagedResult<RecallDto>(items, paged.TotalCount, paged.PageNumber, paged.PageSize);
    }
}
