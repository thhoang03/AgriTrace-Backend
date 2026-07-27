using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

/// <summary>
/// Request body for creating a user. Matches swagger <c>CreateUserRequest</c>.
/// organizationId is not accepted; it is auto-assigned from the authenticated MANAGER's token.
/// role is a required string enum (STAFF only).
/// </summary>
public class CreateUserRequest
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}
