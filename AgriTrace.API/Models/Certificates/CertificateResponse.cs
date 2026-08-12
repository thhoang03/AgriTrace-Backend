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

    [JsonPropertyName("certificateNumber")]
    public string? CertificateNumber { get; set; }

    [JsonPropertyName("certificateType")]
    public string CertificateType { get; set; } = string.Empty;

    [JsonPropertyName("issuingOrganization")]
    public string? IssuingOrganization { get; set; }

    [JsonPropertyName("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("issuedDate")]
    public DateOnly? IssuedDate { get; set; }

    [JsonPropertyName("expiryDate")]
    public DateOnly? ExpiryDate { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; } = 3;

    [JsonPropertyName("statusName")]
    public string StatusName { get; set; } = "Active";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

