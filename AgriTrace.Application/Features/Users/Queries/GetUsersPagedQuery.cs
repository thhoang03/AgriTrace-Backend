using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Domain.Common;
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

        if (request.OrganizationId.HasValue && request.OrganizationId.Value != Guid.Empty)
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
            .OrderByDescending(x => x.CreatedAt)
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

