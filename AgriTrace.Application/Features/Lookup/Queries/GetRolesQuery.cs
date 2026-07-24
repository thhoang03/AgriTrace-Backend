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

public record GetRolesQuery : IRequest<IReadOnlyList<LookupItemDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IReadOnlyList<LookupItemDto>>
{
    public Task<IReadOnlyList<LookupItemDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<LookupItemDto> items = Enum.GetValues<UserRole>()
            .Select(role => new LookupItemDto
            {
                Id = ((int)role).ToString(),
                Code = role.ToString().ToUpperInvariant(),
                Name = role.ToString()
            })
            .ToList();

        return Task.FromResult(items);
    }
}

