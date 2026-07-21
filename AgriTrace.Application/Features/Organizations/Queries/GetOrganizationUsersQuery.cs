using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

/// <summary>
/// Returns the users belonging to an organization. STUB for Phase 6 — the Users feature (Phase 7)
/// will implement the real handler. For now it returns an empty page.
/// </summary>
public sealed record GetOrganizationUsersQuery(
    Guid OrganizationId,
    int Page,
    int PageSize)
    : IRequest<PagedResult<UserDto>>;

public sealed class GetOrganizationUsersQueryHandler
    : IRequestHandler<GetOrganizationUsersQuery, PagedResult<UserDto>>
{
    public Task<PagedResult<UserDto>> Handle(
        GetOrganizationUsersQuery query,
        CancellationToken cancellationToken)
    {
        // TODO Phase 7: query users by organization from the Users read service.
        return Task.FromResult(
            new PagedResult<UserDto>(
                Array.Empty<UserDto>(),
                0,
                query.Page,
                query.PageSize));
    }
}
