using System.Text.Json.Serialization;

namespace AgriTrace.Domain.Entities.Organizations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrganizationStatus
{
    Active = 1,
    Inactive = 2
}
