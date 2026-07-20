using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Users.Queries;

public record GetUsersPagedQuery(
    Guid? OrganizationId,
    string? Role,
    string? Search,
    int Page,
    int PageSize) : IRequest<PagedResult<UserDto>>;

public class GetUsersPagedQueryHandler : IRequestHandler<GetUsersPagedQuery, PagedResult<UserDto>>
{
    private readonly IUserService _userService;

    public GetUsersPagedQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersPagedQuery request, CancellationToken cancellationToken)
    {
        var all = await _userService.GetAllAsync(cancellationToken);

        var query = all.AsEnumerable();

        if (request.OrganizationId.HasValue)
        {
            query = query.Where(x => x.OrganizationId == request.OrganizationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Role)
            && Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            query = query.Where(x => x.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.FullName.ToLowerInvariant().Contains(search)
                || x.Email.ToLowerInvariant().Contains(search));
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(x => x.Email)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(CreateUserCommandHandler.ToDto)
            .ToList();

        return new PagedResult<UserDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
