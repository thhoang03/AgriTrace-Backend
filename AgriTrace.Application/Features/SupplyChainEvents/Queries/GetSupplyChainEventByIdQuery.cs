using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.SupplyChainEvents.Commands;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.SupplyChainEvents.Queries;

/// <summary>
/// Get a single supply chain event by its Id.
/// </summary>
public sealed record GetSupplyChainEventByIdQuery(Guid EventId)
    : IRequest<SupplyChainEventDto?>;

public sealed class GetSupplyChainEventByIdQueryHandler
    : IRequestHandler<GetSupplyChainEventByIdQuery, SupplyChainEventDto?>
{
    private readonly ISupplyChainEventRepository _repository;

    public GetSupplyChainEventByIdQueryHandler(ISupplyChainEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplyChainEventDto?> Handle(
        GetSupplyChainEventByIdQuery query,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(query.EventId, cancellationToken);

        return entity is null ? null : CreateSupplyChainEventCommandHandler.ToDto(entity);
    }
}
