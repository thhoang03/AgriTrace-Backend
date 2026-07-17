namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a certificate record.
/// </summary>
public class CertificateResponse
{
    public Guid CertificateId { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchCode { get; set; }
    public Guid? InspectionId { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime? IssuedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
