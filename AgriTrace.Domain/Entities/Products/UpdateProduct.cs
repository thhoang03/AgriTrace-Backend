namespace AgriTrace.Domain.Entities.Products;

public sealed record UpdateProduct(
    Guid? CategoryId,
    Guid? UnitId,
    string Name,
    Guid? OrganizationId = null);
