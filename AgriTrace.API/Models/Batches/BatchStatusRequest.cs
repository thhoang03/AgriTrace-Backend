using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for setting a batch's status. Matches swagger <c>BatchStatusRequest</c>.
/// </summary>
public class BatchStatusRequest
{
    [Required]
    public int Status { get; set; }
}
