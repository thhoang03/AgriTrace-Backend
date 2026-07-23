using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

/// <summary>
/// Request body for changing a user's active status. Matches swagger <c>UserStatusRequest</c>.
/// </summary>
public class UserStatusRequest
{
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
