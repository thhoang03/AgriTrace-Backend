using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Queries;

public sealed record GetQualityInspectionsPagedQuery(
    int Page,
    int PageSize)
    : IRequest<PagedResult<QualityInspectionDto>>;

public sealed class GetQualityInspectionsPagedQueryHandler
    : IRequestHandler<GetQualityInspectionsPagedQuery, PagedResult<QualityInspectionDto>>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionsPagedQueryHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<PagedResult<QualityInspectionDto>> Handle(
        GetQualityInspectionsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var paged = await _service.GetPagedAsync(query.Page, query.PageSize, cancellationToken);

        var items = paged.Items
            .Select(i => new QualityInspectionDto
            {
                Id = i.Id,
                BatchId = i.BatchId,
                BatchCode = i.Batch?.BatchCode,
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

        return new PagedResult<QualityInspectionDto>(
            items, paged.TotalCount, query.Page, query.PageSize);
    }
}
