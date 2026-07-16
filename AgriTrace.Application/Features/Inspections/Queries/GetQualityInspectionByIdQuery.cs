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
        var inspection = await _service.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (inspection is null)
        {
            return null;
        }

        return new QualityInspectionDto
        {
            Id = inspection.Id,
            BatchId = inspection.BatchId,
            InspectorId = inspection.InspectorId,
            Status = (int)inspection.Status,
            Result = inspection.Result,
            Notes = inspection.Notes,
            CreatedAt = inspection.CreatedAt,
            UpdatedAt = inspection.UpdatedAt
        };
    }
}
