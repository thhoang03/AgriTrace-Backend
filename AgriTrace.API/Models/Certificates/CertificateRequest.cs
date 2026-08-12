using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for issuing a new certificate on a batch.
/// </summary>
public class IssueCertificateRequest
{
    /// <summary>
    /// Optional related inspection Id.
    /// </summary>
    public Guid? InspectionId { get; set; }

    /// <summary>
    /// Certificate number (e.g. VG-2026-00125).
    /// </summary>
    public string? CertificateNumber { get; set; }

    /// <summary>
    /// Certificate type (VietGAP, GlobalGAP, Organic, HACCP, ISO 22000...).
    /// </summary>
    [Required]
    public string CertificateType { get; set; } = null!;

    /// <summary>
    /// Organization issuing the certificate.
    /// </summary>
    public string? IssuingOrganization { get; set; }

    /// <summary>
    /// URL to the certificate file document.
    /// </summary>
    [Required]
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// Issued date.
    /// </summary>
    [Required]
    public DateOnly IssuedDate { get; set; }

    /// <summary>
    /// Expiry date.
    /// </summary>
    public DateOnly? ExpiryDate { get; set; }

    /// <summary>
    /// Notes or Scope.
    /// </summary>
    public string? Notes { get; set; }
}

