using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

public class AdminResetPasswordRequest
{
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}
