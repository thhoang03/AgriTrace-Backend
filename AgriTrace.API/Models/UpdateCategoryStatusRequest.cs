using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public class UpdateCategoryStatusRequest
{
    [Required]
    public bool IsActive { get; set; }
}
