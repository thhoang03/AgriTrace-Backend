using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Recalls.Commands;

public record ResolveRecallCommand(
    Guid RecallId,
    int Status,
    Guid ResolvedByUserId) : IRequest<MediatR.Unit>;

public class ResolveRecallCommandHandler : IRequestHandler<ResolveRecallCommand, MediatR.Unit>
{
    private readonly IRecallService _recallService;

    public ResolveRecallCommandHandler(IRecallService recallService)
    {
        _recallService = recallService;
    }

    public async Task<MediatR.Unit> Handle(ResolveRecallCommand request, CancellationToken cancellationToken)
    {
        var recall = await _recallService.GetByIdAsync(request.RecallId, cancellationToken)
            ?? throw new NotFoundException($"Recall {request.RecallId} not found.");

        // Map the incoming status code to the domain transition.
        switch ((RecallStatus)request.Status)
        {
            case RecallStatus.Processing:
                recall.StartProcessing();
                break;
            case RecallStatus.Completed:
                recall.Complete();
                break;
            case RecallStatus.Cancelled:
                recall.Cancel();
                break;
            case RecallStatus.Pending:
                // No-op: already pending on creation.
                break;
            default:
                throw new ArgumentException($"Status '{request.Status}' is invalid.");
        }

        await _recallService.UpdateAsync(recall, cancellationToken);

        return MediatR.Unit.Value;
    }
}
