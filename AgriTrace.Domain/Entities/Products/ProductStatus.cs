using System.Text.Json.Serialization;

namespace AgriTrace.Domain.Entities.Products;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductStatus
{
    Created = 0,
    Active = 1,
    Inactive = 2
}

