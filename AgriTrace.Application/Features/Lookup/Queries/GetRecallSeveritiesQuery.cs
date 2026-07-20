using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common.Enums;
using MediatR;

namespace AgriTrace.Application.Features.Lookup.Queries;

public record GetRecallSeveritiesQuery : IRequest<IReadOnlyList<LookupItemDto>>;

public class GetRecallSeveritiesQueryHandler : IRequestHandler<GetRecallSeveritiesQuery, IReadOnlyList<LookupItemDto>>
{
    public Task<IReadOnlyList<LookupItemDto>> Handle(GetRecallSeveritiesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<LookupItemDto> items = Enum.GetValues<RecallSeverity>()
            .Select(severity => new LookupItemDto
            {
                Id = ((int)severity).ToString(),
                Code = severity.ToString().ToUpperInvariant(),
                Name = severity.ToString()
            })
            .ToList();

        return Task.FromResult(items);
    }
}
