using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a batch QR code. Matches swagger <c>QrCodeData</c>.
/// </summary>
public class QrCodeData
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = null!;

    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; set; }

    [JsonPropertyName("publicTraceUrl")]
    public string PublicTraceUrl { get; set; } = null!;
}
