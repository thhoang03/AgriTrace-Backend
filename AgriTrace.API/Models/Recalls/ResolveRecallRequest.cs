using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Recalls;

/// <summary>
/// Request body for resolving a recall. Matches swagger <c>ResolveRecallRequest</c>.
/// </summary>
public class ResolveRecallRequest
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}
