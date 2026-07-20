using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Lookup;

/// <summary>
/// Generic reference-data item. Matches swagger <c>LookupItem</c>.
/// </summary>
public class LookupItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
