using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Events.Queries;

/// <summary>
/// Result of hash chain verification. Mirrors swagger verify response { isValid, totalEvents }.
/// </summary>
public class HashChainVerificationResult
{
    public bool IsValid { get; set; }

    public int TotalEvents { get; set; }
}

public record VerifyHashChainQuery(
    Guid BatchId) : IRequest<HashChainVerificationResult>;

public class VerifyHashChainQueryHandler : IRequestHandler<VerifyHashChainQuery, HashChainVerificationResult>
{
    private readonly IEventService _eventService;

    public VerifyHashChainQueryHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<HashChainVerificationResult> Handle(VerifyHashChainQuery request, CancellationToken cancellationToken)
    {
        var isValid = await _eventService.VerifyHashChainAsync(request.BatchId, cancellationToken);
        var events = await _eventService.GetByBatchAsync(request.BatchId, cancellationToken);

        return new HashChainVerificationResult
        {
            IsValid = isValid,
            TotalEvents = events.Count
        };
    }
}
