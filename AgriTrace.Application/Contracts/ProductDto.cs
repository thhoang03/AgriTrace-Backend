public sealed class ProductDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public Guid OrganizationId { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? UnitId { get; init; }

    public string? CategoryName { get; init; }

    public string? UnitName { get; init; }

    /// <summary>
    /// Active flag for the product. The domain <c>Product</c> entity does not yet expose a Status
    /// field, so this defaults to Created until a domain property is introduced (Phase 11 follow-up).
    /// </summary>
    public ProductStatus Status { get; set; } = ProductStatus.Created;
}