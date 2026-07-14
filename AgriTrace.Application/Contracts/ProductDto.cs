public sealed class ProductDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public Guid OrganizationId { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? UnitId { get; init; }

    public string? CategoryName { get; init; }

    public string? UnitName { get; init; }
}