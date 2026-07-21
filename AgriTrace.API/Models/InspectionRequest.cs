using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for creating a new quality inspection on a batch.
/// </summary>
public class CreateInspectionRequest
{
    /// <summary>
    /// Inspection result: "PASS" or "FAIL".
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string Result { get; set; } = null!;

    /// <summary>
    /// Optional notes about the inspection findings.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Request body for updating an existing quality inspection.
/// </summary>
public class UpdateInspectionRequest
{
    /// <summary>
    /// Updated inspection result: "PASS" or "FAIL".
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string Result { get; set; } = null!;

    /// <summary>
    /// Updated notes about the inspection findings.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
}
