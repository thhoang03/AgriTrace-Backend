using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Batches.Commands;

/// <summary>
/// Result of a merge. Mirrors swagger <c>MergeBatchData</c>.
/// </summary>
public class MergeBatchResult
{
    public Guid NewBatchId { get; set; }

    public string BatchCode { get; set; } = string.Empty;
}

public record MergeBatchCommand(
    List<Guid> SourceBatchIds,
    Guid ProductId,
    decimal Quantity,
    Guid UnitId,
    DateOnly ProductionDate) : IRequest<MergeBatchResult>;

public class MergeBatchCommandHandler : IRequestHandler<MergeBatchCommand, MergeBatchResult>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IBatchMergeRepository _mergeRepository;

    public MergeBatchCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IBatchMergeRepository mergeRepository)
    {
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _mergeRepository = mergeRepository;
    }

    public async Task<MergeBatchResult> Handle(MergeBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.SourceBatchIds is null || request.SourceBatchIds.Count < 2)
        {
            throw new ArgumentException("A merge requires at least two source batches.");
        }

        // Load and validate source batches.
        var sources = new List<Batch>();
        foreach (var id in request.SourceBatchIds)
        {
            var source = await _batchReadService.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Source batch {id} not found.");
            sources.Add(source);
        }

        var batchCode = "MERGED-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        var productionDate = request.ProductionDate.ToDateTime(TimeOnly.MinValue);

        var newBatch = new Batch(
            request.ProductId,
            batchCode,
            request.Quantity,
            request.UnitId,
            productionDate,
            expiryDate: null);

        // The merged batch inherits the organization of the first source batch.
        newBatch.ChangeOrganization(sources[0].CurrentOrganizationId);

        await _batchWriteService.CreateAsync(newBatch, cancellationToken);

        var merge = new BatchMerge(newBatch.Id);
        foreach (var source in sources)
        {
            merge.AddSource(source.Id, source.RemainingQuantity);
        }

        await _mergeRepository.AddAsync(merge, cancellationToken);

        return new MergeBatchResult
        {
            NewBatchId = newBatch.Id,
            BatchCode = newBatch.BatchCode
        };
    }
}
