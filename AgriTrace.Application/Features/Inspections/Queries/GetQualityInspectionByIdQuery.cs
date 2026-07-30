using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Queries;

public sealed record GetQualityInspectionByIdQuery(Guid Id)
    : IRequest<QualityInspectionDto?>;

public sealed class GetQualityInspectionByIdQueryHandler
    : IRequestHandler<GetQualityInspectionByIdQuery, QualityInspectionDto?>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionByIdQueryHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<QualityInspectionDto?> Handle(
        GetQualityInspectionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var inspection = await _service.GetByIdWithLabTestsAsync(
            query.Id, cancellationToken);

        if (inspection is null) return null;

        return MapToDto(inspection);
    }

    internal static QualityInspectionDto MapToDto(
        Domain.Entities.QualityInspections.QualityInspection inspection)
    {
        return new QualityInspectionDto
        {
            Id = inspection.Id,
            BatchId = inspection.BatchId,
            BatchCode = inspection.Batch?.BatchCode,
            InspectorId = inspection.InspectorId,
            InspectorName = inspection.Inspector?.FullName,
            InspectionType = (int)inspection.InspectionType,
            Status = (int)inspection.Status,
            OverallResult = inspection.OverallResult,
            InspectionDate = inspection.InspectionDate,
            Notes = inspection.Notes,
            CreatedAt = inspection.CreatedAt,
            UpdatedAt = inspection.UpdatedAt,
            LabTests = inspection.LabTests
                .Select(t => new InspectionLabTestDto
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
                })
                .ToList()
        };
    }
}
