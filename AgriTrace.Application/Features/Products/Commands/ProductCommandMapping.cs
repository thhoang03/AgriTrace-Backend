using AgriTrace.Domain.Entities.Products;

namespace AgriTrace.Application.Features.Products.Commands;

internal static class ProductCommandMapping
{
    public static NewProduct ToNewProduct(
        this CreateProductCommand command)
    {
        return new NewProduct(
            OrganizationId: command.OrganizationId,
            CategoryId: command.CategoryId,
            UnitId: command.UnitId,
            Name: command.Name);
    }

    public static UpdateProduct ToUpdateProduct(
        this UpdateProductCommand command)
    {
        return new UpdateProduct(
            CategoryId: command.CategoryId,
            UnitId: command.UnitId,
            Name: command.Name);
    }
}