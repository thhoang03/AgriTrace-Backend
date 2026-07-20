namespace AgriTrace.API.Services;

/// <summary>
/// Exposes the authenticated caller's identity extracted from the current request's JWT claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>The authenticated user's id. Throws <see cref="UnauthorizedAccessException"/> when unauthenticated or unparsable.</summary>
    Guid UserId { get; }

    /// <summary>The authenticated user's role claim, or null when unauthenticated.</summary>
    string? Role { get; }

    /// <summary>The authenticated user's organization id, or null when absent.</summary>
    Guid? OrganizationId { get; }

    /// <summary>Whether the current request carries an authenticated identity.</summary>
    bool IsAuthenticated { get; }
}
