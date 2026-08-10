namespace AgriTrace.Infrastructure.Sqlserver.Models;

public class CertificateDataModel : BaseDataModel
{
    // FK Batch
    public Guid BatchId { get; set; }

    // FK QualityInspection (optional)
    public Guid? InspectionId { get; set; }

    public string? CertificateNumber { get; set; }

    public string? CertificateType { get; set; }

    public string? IssuingOrganization { get; set; }

    public string? FileUrl { get; set; }

    public DateTime? IssuedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public int Status { get; set; } = 3;

    public string? Notes { get; set; }

    // Navigation
    public BatchDataModel Batch { get; set; } = null!;

    public QualityInspectionDataModel? Inspection { get; set; }
}