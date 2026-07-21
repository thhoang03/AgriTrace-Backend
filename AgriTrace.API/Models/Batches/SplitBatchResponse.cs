using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Batches;

/// <summary>
/// Response data for a batch split. Matches swagger <c>SplitBatchData</c>.
/// </summary>
public class SplitBatchResponse
{
    [JsonPropertyName("parentBatchId")]
    public Guid ParentBatchId { get; set; }

    [JsonPropertyName("childBatchIds")]
    public List<Guid> ChildBatchIds { get; set; } = new();
}
