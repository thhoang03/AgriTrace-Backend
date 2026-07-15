namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a quality inspection record.
/// </summary>
public class InspectionResponse
{
    public Guid InspectionId { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchCode { get; set; }
    public Guid InspectorId { get; set; }
    public string? InspectorName { get; set; }

    /// <summary>
    /// Numeric status: 1 = Pending, 2 = Passed, 3 = Failed.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// String result: "PASS", "FAIL", or null when still Pending.
    /// </summary>
    public string? Result { get; set; }

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
