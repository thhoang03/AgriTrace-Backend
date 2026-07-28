using System.Security.Claims;
using AgriTrace.Domain.Interfaces.Inbound;
using Microsoft.AspNetCore.Http;

namespace AgriTrace.Infrastructure.Sqlserver.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue("sub");

            if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
            {
                throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
            }

            return id;
        }
    }

    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role) ?? User?.FindFirstValue("role");

    public Guid? OrganizationId
    {
        get
        {
            var value = User?.FindFirstValue("organizationId");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? OrganizationType =>
        User?.FindFirstValue("organizationType");
}
