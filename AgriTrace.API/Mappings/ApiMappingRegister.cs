using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Application.Features.Products.Commands;

namespace AgriTrace.API.Mapping;

internal static class ApiMappings
{
    // =========================
    // Request -> Command
    // =========================

    //=======PRODUCT=======
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
    //========ORGANIZATION=======
    public static CreateOrganizationCommand ToCommand(this OrganizationRequest request)
    {
        return new CreateOrganizationCommand(
            request.OrganizationTypeId,
            request.OrganizationName,
            request.Address);
    }
    public static UpdateOrganizationCommand ToCommand(this OrganizationRequest request, Guid id)
    {
        return new UpdateOrganizationCommand(
            id,
            request.OrganizationTypeId,
            request.OrganizationName,
            request.Address);
    }

    // =========================
    // DTO -> Response
    // =========================

    //=======PRODUCT=======
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
    //=======ORGANIZATION========
    public static OrgranizationResponse ToResponse(this OrganizationDto dto)
    {
        return new OrgranizationResponse
        {
            OrganizationId = dto.Id,
            OrganizationName = dto.Name,
            Address = dto.Address ?? string.Empty,
            OrganizationTypeId = dto.OrganizationTypeId,
            OrganizationStatus = dto.Status.ToString(),
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}