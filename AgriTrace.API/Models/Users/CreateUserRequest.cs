using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

/// <summary>
/// Request body for creating a user.
/// organizationId: required for ADMIN (select org), ignored for MANAGER (auto-assigned from token).
/// role: ADMIN can set MANAGER/STAFF; MANAGER is forced to STAFF.
/// </summary>
public class CreateUserRequest
{
    [JsonPropertyName("organizationId")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}
