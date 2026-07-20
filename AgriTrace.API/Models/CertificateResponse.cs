using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a certificate record. Matches swagger <c>CertificateDetail</c>.
/// </summary>
public class CertificateResponse
{
    [JsonPropertyName("certificateId")]
    public Guid CertificateId { get; set; }

    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string? BatchCode { get; set; }

    [JsonPropertyName("inspectionId")]
    public Guid? InspectionId { get; set; }

    [JsonPropertyName("certificateType")]
    public string CertificateType { get; set; } = string.Empty;

    [JsonPropertyName("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("issuedDate")]
    public DateOnly? IssuedDate { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
