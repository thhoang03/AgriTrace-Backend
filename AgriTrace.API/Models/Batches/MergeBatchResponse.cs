using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Batches;

/// <summary>
/// Response data for a batch merge. Matches swagger <c>MergeBatchData</c>.
/// </summary>
public class MergeBatchResponse
{
    [JsonPropertyName("newBatchId")]
    public Guid NewBatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = string.Empty;
}
