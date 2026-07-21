using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for issuing a new certificate on a batch.
/// </summary>
public class IssueCertificateRequest
{
    /// <summary>
    /// Related inspection Id.
    /// </summary>
    [Required]
    public Guid InspectionId { get; set; }

    /// <summary>
    /// Certificate type.
    /// </summary>
    [Required]
    public string CertificateType { get; set; } = null!;

    /// <summary>
    /// URL to the certificate file.
    /// </summary>
    [Required]
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// Issued date.
    /// </summary>
    [Required]
    public DateOnly IssuedDate { get; set; }
}
