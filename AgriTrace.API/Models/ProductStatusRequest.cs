using System.Text.Json.Serialization;
using AgriTrace.Domain.Entities.Products;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for setting a product's status.
/// Supports both enum Status and boolean IsActive fields.
/// </summary>
public class ProductStatusRequest
{
    [JsonPropertyName("status")]
    public ProductStatus? Status { get; set; }

    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }
}

