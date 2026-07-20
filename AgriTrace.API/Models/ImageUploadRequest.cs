using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for uploading an image. Matches swagger <c>ImageUploadRequest</c>.
/// </summary>
public class ImageUploadRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    public bool? IsPrimary { get; set; }
}
