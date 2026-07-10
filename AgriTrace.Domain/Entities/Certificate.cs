using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid? InspectionId { get; private set; }

    public string CertificateType { get; private set; }

    public string FileUrl { get; private set; }

    public DateTime? IssuedDate { get; private set; }

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
        DateTime? issuedDate)
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
    }

    public void UpdateInformation(
        string certificateType,
        string fileUrl,
        Guid? inspectionId)
    {
        Validate(
            BatchId,
            certificateType,
            fileUrl);

        CertificateType = certificateType.Trim();
        FileUrl = fileUrl.Trim();
        InspectionId = inspectionId;

        MarkUpdated();
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