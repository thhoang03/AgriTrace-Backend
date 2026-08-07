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
        var inspections = await _service.GetByBatchAsync(query.BatchId, cancellationToken);

        var all = inspections
            .Select(i => new QualityInspectionDto
            {
                Id = i.Id,
                BatchId = i.BatchId,
                BatchCode = i.Batch?.BatchCode,
                OrganizationId = i.OrganizationId,
                InspectorId = i.InspectorId,
                InspectorName = i.Inspector?.FullName,
                InspectionType = (int)i.InspectionType,
                Status = (int)i.Status,
                OverallResult = i.OverallResult,
                InspectionDate = i.InspectionDate,
                Notes = i.Notes,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                LabTests = i.LabTests.Select(t => new InspectionLabTestDto
                {
                    Id = t.Id,
                    InspectionId = t.InspectionId,
                    TestName = t.TestName,
                    MeasuredValue = t.MeasuredValue,
                    Unit = t.Unit,
                    MinStandardValue = t.MinStandardValue,
                    MaxStandardValue = t.MaxStandardValue,
                    IsPassed = t.IsPassed,
                    Remark = t.Remark,
                    CreatedAt = t.CreatedAt
                }).ToList()
            })
            .ToList();

        var pageItems = all
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagedResult<QualityInspectionDto>(
            pageItems, all.Count, query.Page, query.PageSize);
    }
}
