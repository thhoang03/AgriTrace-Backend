using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for toggling active status. Matches swagger <c>ActiveStatusRequest</c>.
/// </summary>
public class ActiveStatusRequest
{
    [Required]
    public bool IsActive { get; set; }
}
