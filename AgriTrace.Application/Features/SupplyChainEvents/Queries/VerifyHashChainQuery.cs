using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.SupplyChainEvents.Queries;

/// <summary>
/// Verifies that the hash chain for a batch is intact (tamper-proof check).
/// Returns true if every event hash is valid, false if the chain is broken.
/// </summary>
public sealed record VerifyHashChainQuery(Guid BatchId) : IRequest<bool>;

public sealed class VerifyHashChainQueryHandler
    : IRequestHandler<VerifyHashChainQuery, bool>
{
    private readonly IEventService _service;

    public VerifyHashChainQueryHandler(IEventService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(
        VerifyHashChainQuery query,
        CancellationToken cancellationToken)
    {
        return await _service.VerifyHashChainAsync(query.BatchId, cancellationToken);
    }
}
