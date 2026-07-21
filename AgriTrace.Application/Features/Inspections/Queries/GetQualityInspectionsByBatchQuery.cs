using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Queries;

public sealed record GetQualityInspectionsByBatchQuery(
    Guid BatchId,
    int Page,
    int PageSize)
    : IRequest<PagedResult<QualityInspectionDto>>;

public sealed class GetQualityInspectionsByBatchQueryHandler
    : IRequestHandler<GetQualityInspectionsByBatchQuery, PagedResult<QualityInspectionDto>>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionsByBatchQueryHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<PagedResult<QualityInspectionDto>> Handle(
        GetQualityInspectionsByBatchQuery query,
        CancellationToken cancellationToken)
    {
        var inspections = await _service.GetByBatchAsync(
            query.BatchId,
            cancellationToken);

        var all = inspections
            .Select(i => new QualityInspectionDto
            {
                Id = i.Id,
                BatchId = i.BatchId,
                InspectorId = i.InspectorId,
                Status = (int)i.Status,
                Result = i.Result,
                Notes = i.Notes,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToList();

        var pageItems = all
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagedResult<QualityInspectionDto>(
            pageItems,
            all.Count,
            query.Page,
            query.PageSize);
    }
}
