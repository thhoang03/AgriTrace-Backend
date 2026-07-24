using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetBatchDistributionQuery(
    Guid? OrganizationId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<BatchDistributionDto>;

public class GetBatchDistributionQueryHandler : IRequestHandler<GetBatchDistributionQuery, BatchDistributionDto>
{
    private readonly IBatchReadService _batchReadService;

    public GetBatchDistributionQueryHandler(IBatchReadService batchReadService)
    {
        _batchReadService = batchReadService;
    }

    public async Task<BatchDistributionDto> Handle(GetBatchDistributionQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Batch> batches = request.OrganizationId.HasValue
            ? await _batchReadService.GetByOrganizationAsync(request.OrganizationId.Value, cancellationToken)
            : await _batchReadService.GetAllAsync(cancellationToken);

        var filtered = batches.Where(b =>
            (!request.FromDate.HasValue || b.CreatedAt >= request.FromDate.Value) &&
            (!request.ToDate.HasValue || b.CreatedAt <= request.ToDate.Value))
            .ToList();

        var items = filtered
            .GroupBy(b => b.Status)
            .Select(g => new BatchStatusDistributionItemDto
            {
                Status = (int)g.Key,
                StatusName = g.Key.ToString().ToUpperInvariant(),
                Count = g.Count()
            })
            .OrderBy(i => i.Status)
            .ToList();

        return new BatchDistributionDto
        {
            Items = items,
            TotalCount = filtered.Count
        };
    }
}

