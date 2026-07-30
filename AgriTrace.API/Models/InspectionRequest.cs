using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public class CreateInspectionRequest
{
    [Required]
    [Range(1, 7)]
    public int InspectionType { get; set; }

    [Required]
    public DateTime InspectionDate { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }
}
