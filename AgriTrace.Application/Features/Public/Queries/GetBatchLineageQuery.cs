using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Public.Queries;

public record GetBatchLineageQuery(Guid BatchId) : IRequest<LineageDto>;

public class GetBatchLineageQueryHandler : IRequestHandler<GetBatchLineageQuery, LineageDto>
{
    private readonly IBatchReadService _batchReadService;

    public GetBatchLineageQueryHandler(IBatchReadService batchReadService)
    {
        _batchReadService = batchReadService;
    }

    public async Task<LineageDto> Handle(GetBatchLineageQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        // Determine the root of the lineage tree.
        var rootBatchId = batch.RootBatchId ?? batch.Id;
        var root = await _batchReadService.GetByIdAsync(rootBatchId, cancellationToken) ?? batch;

        // Iterative (queue-based) breadth-first traversal to avoid deep recursion on large trees.
        var nodes = new List<LineageNodeDto>();
        var visited = new HashSet<Guid>();
        var queue = new Queue<Batch>();
        queue.Enqueue(root);
        visited.Add(root.Id);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            nodes.Add(new LineageNodeDto
            {
                BatchId = current.Id,
                BatchCode = current.BatchCode,
                // A batch that has a parent was produced by a split from that parent.
                EventTypeCode = current.ParentBatchId.HasValue ? "SPLIT" : null,
                Quantity = current.Quantity,
                ParentBatchId = current.ParentBatchId
            });

            var children = await _batchReadService.GetByParentBatchAsync(current.Id, cancellationToken);
            foreach (var child in children)
            {
                if (visited.Add(child.Id))
                {
                    queue.Enqueue(child);
                }
            }
        }

        return new LineageDto
        {
            RootBatchId = rootBatchId,
            Lineage = nodes
        };
    }
}
