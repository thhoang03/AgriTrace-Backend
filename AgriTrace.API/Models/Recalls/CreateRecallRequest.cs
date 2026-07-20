using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Recalls;

/// <summary>
/// Request body for creating a recall. Matches swagger <c>CreateRecallRequest</c>.
/// severity: 1=LOW, 2=MEDIUM, 3=HIGH.
/// </summary>
public class CreateRecallRequest
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [Required]
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [Range(1, 3)]
    [JsonPropertyName("severity")]
    public int Severity { get; set; }
}
