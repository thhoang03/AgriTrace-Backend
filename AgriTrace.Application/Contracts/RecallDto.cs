namespace AgriTrace.Application.Contracts;

/// <summary>
/// DTO for a recall record. Mirrors swagger <c>RecallDetail</c>.
/// </summary>
public class RecallDto
{
    public Guid RecallId { get; set; }

    public Guid BatchId { get; set; }

    public string? BatchCode { get; set; }

    public Guid? ProductId { get; set; }

    public string? ProductName { get; set; }

    public Guid? OrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public Guid CreatedBy { get; set; }

    public string? CreatedByName { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int Severity { get; set; }

    public string SeverityName { get; set; } = string.Empty;

    public int Status { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
