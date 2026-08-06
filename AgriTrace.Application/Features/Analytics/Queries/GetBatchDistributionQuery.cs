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
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Models.Analytics;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetBatchDistributionQuery(
    Guid? OrganizationId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<BatchDistributionDto>;

public class GetBatchDistributionQueryHandler : IRequestHandler<GetBatchDistributionQuery, BatchDistributionDto>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetBatchDistributionQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<BatchDistributionDto> Handle(GetBatchDistributionQuery request, CancellationToken cancellationToken)
    {
        var result = await _analyticsRepository.GetBatchDistributionAsync(request.OrganizationId, request.FromDate, request.ToDate, cancellationToken);

        return new BatchDistributionDto
        {
            Items = result.Items.Select(i => new BatchStatusDistributionItemDto
            {
                Status = i.Status,
                StatusName = i.StatusName,
                Count = i.Count
            }).ToList(),
            TotalCount = result.TotalCount
        };
    }
}

