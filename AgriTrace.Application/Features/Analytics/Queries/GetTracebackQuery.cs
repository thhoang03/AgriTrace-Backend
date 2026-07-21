using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetTracebackQuery(Guid BatchId) : IRequest<TracebackDto>;

public class GetTracebackQueryHandler : IRequestHandler<GetTracebackQuery, TracebackDto>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchSplitService _batchSplitService;
    private readonly IBatchMergeService _batchMergeService;
    private readonly IOrganizationService _organizationService;
    private readonly IOrganizationTypeService _organizationTypeService;

    public GetTracebackQueryHandler(
        IBatchReadService batchReadService,
        IBatchSplitService batchSplitService,
        IBatchMergeService batchMergeService,
        IOrganizationService organizationService,
        IOrganizationTypeService organizationTypeService)
    {
        _batchReadService = batchReadService;
        _batchSplitService = batchSplitService;
        _batchMergeService = batchMergeService;
        _organizationService = organizationService;
        _organizationTypeService = organizationTypeService;
    }

    public async Task<TracebackDto> Handle(GetTracebackQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        var affected = new List<AffectedBatchDto>();
        var affectedIds = new HashSet<Guid>();
        var relatedBatchIds = new HashSet<Guid> { batch.Id };

        // Batches created by splitting this batch (relationship: SPLIT).
        var splits = await _batchSplitService.GetBySourceBatchAsync(batch.Id, cancellationToken);
        foreach (var target in splits.SelectMany(s => s.Details).Select(d => d.TargetBatchId))
        {
            await AddAffectedAsync(target, "SPLIT", affected, affectedIds, relatedBatchIds, cancellationToken);
        }

        // Merges in which this batch participated as a source (relationship: MERGE).
        var allMerges = await _batchMergeService.GetAllAsync(cancellationToken);
        foreach (var merge in allMerges.Where(m => m.Sources.Any(s => s.SourceBatchId == batch.Id)))
        {
            await AddAffectedAsync(merge.NewBatchId, "MERGE", affected, affectedIds, relatedBatchIds, cancellationToken);
        }

        // Related organizations = distinct organizations across the batch and its affected batches.
        var relatedOrganizations = new List<RelatedOrganizationDto>();
        var seenOrgs = new HashSet<Guid>();
        foreach (var relatedBatchId in relatedBatchIds)
        {
            var relatedBatch = await _batchReadService.GetByIdAsync(relatedBatchId, cancellationToken);
            if (relatedBatch is null || !seenOrgs.Add(relatedBatch.CurrentOrganizationId))
            {
                continue;
            }

            var org = await _organizationService.GetByIdAsync(relatedBatch.CurrentOrganizationId, cancellationToken);
            if (org is null)
            {
                continue;
            }

            var orgType = await _organizationTypeService.GetByIdAsync(org.OrganizationTypeId, cancellationToken);

            relatedOrganizations.Add(new RelatedOrganizationDto
            {
                OrganizationId = org.Id,
                Name = org.Name,
                Type = orgType?.Name
            });
        }

        return new TracebackDto
        {
            BatchId = batch.Id,
            BatchCode = batch.BatchCode,
            AffectedBatches = affected,
            RelatedOrganizations = relatedOrganizations
        };
    }

    private async Task AddAffectedAsync(
        Guid batchId,
        string relationship,
        List<AffectedBatchDto> affected,
        HashSet<Guid> affectedIds,
        HashSet<Guid> relatedBatchIds,
        CancellationToken cancellationToken)
    {
        if (!affectedIds.Add(batchId))
        {
            return;
        }

        var target = await _batchReadService.GetByIdAsync(batchId, cancellationToken);
        if (target is null)
        {
            return;
        }

        relatedBatchIds.Add(batchId);
        affected.Add(new AffectedBatchDto
        {
            BatchId = target.Id,
            BatchCode = target.BatchCode,
            Relationship = relationship
        });
    }
}
