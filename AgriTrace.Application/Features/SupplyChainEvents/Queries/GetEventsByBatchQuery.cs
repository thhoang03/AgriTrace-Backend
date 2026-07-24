using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.SupplyChainEvents.Commands;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.SupplyChainEvents.Queries;

/// <summary>
/// Get all supply chain events for a given batch, ordered by EventTime ascending (timeline).
/// </summary>
public sealed record GetEventsByBatchQuery(Guid BatchId)
    : IRequest<IReadOnlyList<SupplyChainEventDto>>;

public sealed class GetEventsByBatchQueryHandler
    : IRequestHandler<GetEventsByBatchQuery, IReadOnlyList<SupplyChainEventDto>>
{
    private readonly IEventService _service;

    public GetEventsByBatchQueryHandler(IEventService service)
    {
        _service = service;
    }

    public async Task<IReadOnlyList<SupplyChainEventDto>> Handle(
        GetEventsByBatchQuery query,
        CancellationToken cancellationToken)
    {
        var events = await _service.GetByBatchAsync(query.BatchId, cancellationToken);

        return events
            .Select(CreateSupplyChainEventCommandHandler.ToDto)
            .ToList()
            .AsReadOnly();
    }
}
