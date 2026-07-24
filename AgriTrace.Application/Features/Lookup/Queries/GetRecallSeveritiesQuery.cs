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

