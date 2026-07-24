using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Auth;

public class UserBasic
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId => Id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName => Name;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}

public class LoginData
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token => AccessToken;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public UserBasic User { get; set; } = new();
}

public class TokenPair
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token => AccessToken;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
