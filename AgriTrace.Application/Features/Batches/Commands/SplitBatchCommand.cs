using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Batches.Commands;

public record SplitDetailInput(
    decimal Quantity,
    Guid UnitId);

/// <summary>
/// Result of a split. Mirrors swagger <c>SplitBatchData</c>.
/// </summary>
public class SplitBatchResult
{
    public Guid ParentBatchId { get; set; }

    public List<Guid> ChildBatchIds { get; set; } = new();
}

public record SplitBatchCommand(
    Guid BatchId,
    List<SplitDetailInput> Splits,
    Guid PerformedByUserId) : IRequest<SplitBatchResult>;

public class SplitBatchCommandHandler : IRequestHandler<SplitBatchCommand, SplitBatchResult>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IBatchSplitRepository _splitRepository;

    public SplitBatchCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IBatchSplitRepository splitRepository)
    {
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _splitRepository = splitRepository;
    }

    public async Task<SplitBatchResult> Handle(SplitBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.Splits is null || request.Splits.Count < 2)
        {
            throw new ArgumentException("A split requires at least two child batches.");
        }

        var parent = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        var split = new BatchSplit(parent.Id);

        var childIds = new List<Guid>();

        // Create child batches (this reduces the parent's remaining quantity per child).
        foreach (var detail in request.Splits)
        {
            var childCode = Guid.NewGuid().ToString("N")[..8].ToUpper();

            var child = parent.CreateChildBatch(childCode, detail.Quantity, split.Id);

            await _batchWriteService.CreateAsync(child, cancellationToken);

            split.AddDetail(child.Id, detail.Quantity);
            childIds.Add(child.Id);
        }

        // Persist the split audit record and the parent's updated remaining quantity.
        await _splitRepository.AddAsync(split, cancellationToken);
        await _batchWriteService.UpdateAsync(parent, cancellationToken);

        return new SplitBatchResult
        {
            ParentBatchId = parent.Id,
            ChildBatchIds = childIds
        };
    }
}
