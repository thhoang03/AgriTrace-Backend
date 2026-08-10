using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.QualityInspections;

namespace AgriTrace.Domain.Entities.Certificates;

public class Certificate : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid? InspectionId { get; private set; }

    public string? CertificateNumber { get; private set; }

    public string CertificateType { get; private set; }

    public string? IssuingOrganization { get; private set; }

    public string FileUrl { get; private set; }

    public DateTime? IssuedDate { get; private set; }

    public DateTime? ExpiryDate { get; private set; }

    public CertificateStatus Status { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public Batch Batch { get; private set; }

    public QualityInspection? Inspection { get; private set; }

    private Certificate()
    {
    }

    public Certificate(
        Guid batchId,
        Guid? inspectionId,
        string certificateType,
        string fileUrl,
        DateTime? issuedDate,
        string? certificateNumber = null,
        string? issuingOrganization = null,
        DateTime? expiryDate = null,
        CertificateStatus status = CertificateStatus.Active,
        string? notes = null)
    {
        Validate(
            batchId,
            certificateType,
            fileUrl);

        BatchId = batchId;
        InspectionId = inspectionId;
        CertificateType = certificateType.Trim();
        FileUrl = fileUrl.Trim();
        IssuedDate = issuedDate;
        CertificateNumber = certificateNumber?.Trim();
        IssuingOrganization = issuingOrganization?.Trim();
        ExpiryDate = expiryDate;
        Status = status;
        Notes = notes?.Trim();
    }

    public Certificate(
        Guid id,
        Guid batchId,
        Guid? inspectionId,
        string certificateType,
        string fileUrl,
        DateTime? issuedDate,
        DateTime createdAt,
        DateTime? updatedAt,
        string? certificateNumber = null,
        string? issuingOrganization = null,
        DateTime? expiryDate = null,
        CertificateStatus status = CertificateStatus.Active,
        string? notes = null)
        : base(id, createdAt, updatedAt)
    {
        Validate(batchId, certificateType, fileUrl);

        BatchId = batchId;
        InspectionId = inspectionId;
        CertificateType = certificateType.Trim();
        FileUrl = fileUrl.Trim();
        IssuedDate = issuedDate;
        CertificateNumber = certificateNumber?.Trim();
        IssuingOrganization = issuingOrganization?.Trim();
        ExpiryDate = expiryDate;
        Status = status;
        Notes = notes?.Trim();
    }

    public void UpdateInformation(
        string certificateType,
        string fileUrl,
        Guid? inspectionId,
        string? certificateNumber = null,
        string? issuingOrganization = null,
        DateTime? issuedDate = null,
        DateTime? expiryDate = null,
        string? notes = null)
    {
        Validate(
            BatchId,
            certificateType,
            fileUrl);

        CertificateType = certificateType.Trim();
        FileUrl = fileUrl.Trim();
        InspectionId = inspectionId;
        CertificateNumber = certificateNumber?.Trim();
        IssuingOrganization = issuingOrganization?.Trim();
        if (issuedDate.HasValue) IssuedDate = issuedDate;
        if (expiryDate.HasValue) ExpiryDate = expiryDate;
        if (notes != null) Notes = notes.Trim();

        MarkUpdated();
    }

    public void Approve()
    {
        Status = CertificateStatus.Active;
        MarkUpdated();
    }

    public void Reject(string? reason = null)
    {
        Status = CertificateStatus.Rejected;
        if (!string.IsNullOrWhiteSpace(reason)) Notes = reason;
        MarkUpdated();
    }

    public void Suspend(string? reason = null)
    {
        Status = CertificateStatus.Suspended;
        if (!string.IsNullOrWhiteSpace(reason)) Notes = reason;
        MarkUpdated();
    }

    public void CheckAndMarkExpired()
    {
        if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow && Status == CertificateStatus.Active)
        {
            Status = CertificateStatus.Expired;
            MarkUpdated();
        }
    }

    private static void Validate(
        Guid batchId,
        string certificateType,
        string fileUrl)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch is required.");
        }

        if (string.IsNullOrWhiteSpace(certificateType))
        {
            throw new ArgumentException("Certificate type is required.");
        }

        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new ArgumentException("File URL is required.");
        }
    }
}
