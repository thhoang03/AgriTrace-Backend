namespace AgriTrace.Application.Contracts;

public sealed class CertificateDto
{
    public Guid Id { get; init; }
    public Guid BatchId { get; init; }
    public string? BatchCode { get; init; }
    public Guid? InspectionId { get; init; }
    public string? CertificateNumber { get; init; }
    public string CertificateType { get; init; } = string.Empty;
    public string? IssuingOrganization { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public DateTime? IssuedDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public int Status { get; init; } = 3;
    public string StatusName { get; init; } = "Active";
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
