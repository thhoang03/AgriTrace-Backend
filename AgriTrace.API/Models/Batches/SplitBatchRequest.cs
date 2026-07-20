using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Batches;

/// <summary>
/// Request body for splitting a batch. Matches swagger <c>SplitBatchRequest</c> (min 2 splits).
/// </summary>
public class SplitBatchRequest
{
    [Required]
    [MinLength(2)]
    [JsonPropertyName("splits")]
    public List<SplitItem> Splits { get; set; } = new();
}

public class SplitItem
{
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }
}
