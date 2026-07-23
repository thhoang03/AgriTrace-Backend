namespace AgriTrace.Domain.Entities.Products;

public sealed record NewProduct(
    Guid OrganizationId,
    Guid? CategoryId,
    Guid? UnitId,
    string Name);
