using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models.Common;

/// <summary>
/// Request body for setting an organization's status. Matches swagger <c>StatusRequest</c>.
/// </summary>
public class StatusRequest
{
    [Required]
    public string Status { get; set; } = null!;
}
