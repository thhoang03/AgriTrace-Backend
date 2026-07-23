using System.Security.Claims;

namespace AgriTrace.API.Services;

/// <summary>
/// Resolves the current user's identity from <see cref="IHttpContextAccessor"/> and the JWT claims
/// set by the authentication middleware.
/// </summary>
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
            // Depending on JWT claim mapping the id lands under NameIdentifier or the raw "sub" claim.
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
}
