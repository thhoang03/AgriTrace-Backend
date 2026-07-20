using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Batches;

/// <summary>
/// Request body for merging batches. Matches swagger <c>MergeBatchRequest</c> (min 2 sources).
/// </summary>
public class MergeBatchRequest
{
    [Required]
    [MinLength(2)]
    [JsonPropertyName("sourceBatchIds")]
    public List<Guid> SourceBatchIds { get; set; } = new();

    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }

    [JsonPropertyName("productionDate")]
    public DateOnly ProductionDate { get; set; }
}
