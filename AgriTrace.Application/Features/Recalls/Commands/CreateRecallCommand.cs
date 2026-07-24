using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Recalls.Commands;

/// <summary>
/// Result of creating a recall.
/// </summary>
public class RecallCreatedResult
{
    public Guid RecallId { get; set; }
}

public record CreateRecallCommand(
    Guid BatchId,
    string Reason,
    int Severity,
    Guid CreatedByUserId) : IRequest<RecallCreatedResult>;

public class CreateRecallCommandHandler : IRequestHandler<CreateRecallCommand, RecallCreatedResult>
{
    private readonly IRecallService _recallService;
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IUserService _userService;

    public CreateRecallCommandHandler(
        IRecallService recallService,
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IUserService userService
        )
    {
        _recallService = recallService;
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _userService = userService;
    }

    public async Task<RecallCreatedResult> Handle(CreateRecallCommand request, CancellationToken cancellationToken)
    {

        if (request.Severity is < 1 or > 3)
        {
            throw new ArgumentException("Severity must be between 1 and 3.");
        }

        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        if (!batch.CanBeRecalled()) {
            throw new ConflictException($"Batch {request.BatchId} is already under an active recall.");
        }

        // CreatedBy comes from the auth context in Phase 10; fall back to any user until then.
        var createdBy = request.CreatedByUserId;
        if (createdBy == Guid.Empty)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            createdBy = users.FirstOrDefault()?.Id
                ?? throw new ArgumentException(
                    "Cannot create recall without a creating user (no users exist and auth is not yet wired).");
        }

        var recall = new Recall(
            request.BatchId,
            createdBy,
            request.Reason,
            (RecallSeverity)request.Severity);

        var created = await _recallService.CreateAsync(recall, cancellationToken);

        // Setting the batch status to Recalled.
        batch.Recall();
        await _batchWriteService.UpdateAsync(batch, cancellationToken);
        
        return new RecallCreatedResult
        {
            RecallId = created.Id
        };
    }
}
