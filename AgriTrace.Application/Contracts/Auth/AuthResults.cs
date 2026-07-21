namespace AgriTrace.Application.Contracts.Auth;

/// <summary>
/// Basic user info returned after login. Mirrors the swagger <c>UserBasic</c> schema
/// (kept in the Application layer to respect API -&gt; Application dependency direction).
/// </summary>
public class UserBasicInfo
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Result of a successful login. Mirrors the swagger <c>LoginData</c> schema.
/// </summary>
public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public UserBasicInfo User { get; set; } = new();
}

/// <summary>
/// Access + refresh token pair. Mirrors the swagger <c>TokenPair</c> schema.
/// </summary>
public class TokenPairResult
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}
