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

public record GetProcessingTimeQuery(
    Guid? OrganizationId,
    Guid? EventTypeId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<ProcessingTimeDto>;

public class GetProcessingTimeQueryHandler : IRequestHandler<GetProcessingTimeQuery, ProcessingTimeDto>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetProcessingTimeQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ProcessingTimeDto> Handle(GetProcessingTimeQuery request, CancellationToken cancellationToken)
    {
        var result = await _analyticsRepository.GetProcessingTimeAsync(
            request.OrganizationId,
            request.EventTypeId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return new ProcessingTimeDto
        {
            AverageProcessingHours = result.AverageProcessingHours,
            ByEventType = result.ByEventType.Select(x => new ProcessingTimeByEventTypeDto
            {
                EventTypeCode = x.EventTypeCode,
                AverageHours = x.AverageHours
            }).ToList()
        };
    }
}

