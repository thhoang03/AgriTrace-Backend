using MediatR;

namespace AgriTrace.Application.Features.Inspections.Queries;

public sealed record InspectionResultDto(int Value, string Code, string Name);

public sealed record GetInspectionResultsQuery
    : IRequest<IReadOnlyList<InspectionResultDto>>;

public sealed class GetInspectionResultsQueryHandler
    : IRequestHandler<GetInspectionResultsQuery, IReadOnlyList<InspectionResultDto>>
{
    public Task<IReadOnlyList<InspectionResultDto>> Handle(
        GetInspectionResultsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<InspectionResultDto> results = new List<InspectionResultDto>
        {
            new(1, "PENDING", "Pending"),
            new(2, "PASS",    "Passed"),
            new(3, "FAIL",    "Failed")
        };

        return Task.FromResult(results);
    }
}
