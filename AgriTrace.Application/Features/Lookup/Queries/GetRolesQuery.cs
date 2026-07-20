using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common.Enums;
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
