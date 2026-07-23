namespace AgriTrace.Application.Contracts;

/// <summary>
/// Generic reference-data item. Mirrors swagger <c>LookupItem</c>.
/// </summary>
public class LookupItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
