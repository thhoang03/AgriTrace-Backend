using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Queries;

public sealed record GetQualityInspectionsByBatchQuery(Guid BatchId)
    : IRequest<IReadOnlyList<QualityInspectionDto>>;

public sealed class GetQualityInspectionsByBatchQueryHandler
    : IRequestHandler<GetQualityInspectionsByBatchQuery, IReadOnlyList<QualityInspectionDto>>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionsByBatchQueryHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<IReadOnlyList<QualityInspectionDto>> Handle(
        GetQualityInspectionsByBatchQuery query,
        CancellationToken cancellationToken)
    {
        var inspections = await _service.GetByBatchAsync(
            query.BatchId,
            cancellationToken);

        return inspections
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
            .ToList()
            .AsReadOnly();
    }
}
