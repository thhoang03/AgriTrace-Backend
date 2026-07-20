using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Recalls.Queries;

public record GetRecallByIdQuery(
    Guid RecallId) : IRequest<RecallDto>;

public class GetRecallByIdQueryHandler : IRequestHandler<GetRecallByIdQuery, RecallDto>
{
    private readonly IRecallService _recallService;
    private readonly IBatchReadService _batchReadService;
    private readonly IUserService _userService;

    public GetRecallByIdQueryHandler(
        IRecallService recallService,
        IBatchReadService batchReadService,
        IUserService userService)
    {
        _recallService = recallService;
        _batchReadService = batchReadService;
        _userService = userService;
    }

    public async Task<RecallDto> Handle(GetRecallByIdQuery request, CancellationToken cancellationToken)
    {
        var recall = await _recallService.GetByIdAsync(request.RecallId, cancellationToken)
            ?? throw new NotFoundException($"Recall {request.RecallId} not found.");

        return await RecallMapper.ToDtoAsync(recall, _batchReadService, _userService, cancellationToken);
    }
}
