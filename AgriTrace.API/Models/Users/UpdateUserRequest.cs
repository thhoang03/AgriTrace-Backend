using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

/// <summary>
/// Request body for updating a user. Matches swagger <c>UpdateUserRequest</c>.
/// All fields are optional. role is a string enum when provided.
/// </summary>
public class UpdateUserRequest
{
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}
