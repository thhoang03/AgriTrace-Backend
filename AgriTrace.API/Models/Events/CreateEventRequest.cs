using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Events;

/// <summary>
/// Request body for creating a supply-chain event. Matches swagger <c>CreateEventRequest</c>.
/// </summary>
public class CreateEventRequest
{
    [Required]
    [JsonPropertyName("eventTypeId")]
    public Guid EventTypeId { get; set; }

    [JsonPropertyName("eventData")]
    public string? EventData { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }
}
