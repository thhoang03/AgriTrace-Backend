using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Products.Commands;

namespace AgriTrace.API.Mapping;

internal static class ApiMappings
{
    // =========================
    // Request -> Command
    // =========================

    public static CreateProductCommand ToCommand(
        this ProductRequest request)
    {
        return new CreateProductCommand(
            request.OrganizationId,
            request.CategoryId,
            request.UnitId,
            request.Name);
    }

    public static UpdateProductCommand ToCommand(
        this ProductRequest request,
        Guid id)
    {
        return new UpdateProductCommand(
            id,
            request.CategoryId,
            request.UnitId,
            request.Name);
    }

    // =========================
    // DTO -> Response
    // =========================

    public static ProductResponse ToResponse(
        this ProductDto dto)
    {
        return new ProductResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            OrganizationId = dto.OrganizationId,
            CategoryId = dto.CategoryId,
            UnitId = dto.UnitId,
            CategoryName = dto.CategoryName,
            UnitName = dto.UnitName
        };
    }
}