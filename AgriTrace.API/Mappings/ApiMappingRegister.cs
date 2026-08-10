using System;
using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Application.Features.Batches.Commands;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;

namespace AgriTrace.API.Mapping;


internal static class ApiMappings
{


    // =========================
    // Request -> Command
    // =========================



    //=======PRODUCT=======

    private static Guid? ResolveUnitId(Guid? directUnitId, string? unitString)
    {
        if (directUnitId.HasValue && directUnitId.Value != Guid.Empty)
            return directUnitId.Value;

        if (string.IsNullOrWhiteSpace(unitString))
            return new Guid("40000000-0000-0000-0000-000000000001"); // Default to Kilogram

        if (Guid.TryParse(unitString, out var parsedGuid))
            return parsedGuid;

        var normalized = unitString.Trim().ToUpperInvariant();
        return normalized switch
        {
            "KG" or "KILOGRAM" => new Guid("40000000-0000-0000-0000-000000000001"),
            "GRAM" or "G" => new Guid("40000000-0000-0000-0000-000000000002"),
            "LITER" or "L" => new Guid("40000000-0000-0000-0000-000000000003"),
            "MILLILITER" or "ML" => new Guid("40000000-0000-0000-0000-000000000004"),
            "BOX" => new Guid("40000000-0000-0000-0000-000000000005"),
            "BALE" => new Guid("40000000-0000-0000-0000-000000000006"),
            "PIECE" or "PC" or "PACK" or "CRATE" or "BAG" => new Guid("40000000-0000-0000-0000-000000000007"),
            "TON" or "T" or "METRIC TON" => new Guid("40000000-0000-0000-0000-000000000008"),
            "SACK" => new Guid("40000000-0000-0000-0000-000000000009"),
            _ => new Guid("40000000-0000-0000-0000-000000000001")
        };
    }

    public static CreateProductCommand ToCommand(
        this ProductRequest request,
        Guid? fallbackOrgId = null)
    {
        Guid? unitId = ResolveUnitId(request.UnitId, request.Unit);

        var orgId = (request.OrganizationId.HasValue && request.OrganizationId.Value != Guid.Empty)
            ? request.OrganizationId.Value
            : (fallbackOrgId.HasValue && fallbackOrgId.Value != Guid.Empty
                ? fallbackOrgId.Value
                : new Guid("50000000-0000-0000-0000-000000000006"));

        return new CreateProductCommand(
            orgId,
            request.CategoryId,
            unitId,
            request.Name,
            request.Gtin);
    }

    public static UpdateProductCommand ToCommand(
        this ProductRequest request,
        Guid id)
    {
        Guid? unitId = ResolveUnitId(request.UnitId, request.Unit);

        return new UpdateProductCommand(
            id,
            request.CategoryId,
            unitId,
            request.Name,
            request.Gtin);
    }





    //========ORGANIZATION=======


    public static CreateOrganizationCommand ToCommand(
        this OrganizationRequest request)
    {
        return new CreateOrganizationCommand(
            request.OrganizationTypeId,
            request.Name,
            request.Address);
    }

    public static UpdateOrganizationCommand ToCommand(
        this OrganizationRequest request,
        Guid id)
    {
        return new UpdateOrganizationCommand(
            id,
            request.OrganizationTypeId,
            request.Name,
            request.Address);
    }






    //========BATCH=======


    public static CreateBatchCommand ToCommand(
        this CreateBatchRequest request)
    {
        return new CreateBatchCommand(
            request.ProductId,
            request.UnitId,
            request.Quantity,
            request.ProductionDate.ToDateTime(TimeOnly.MinValue),
            request.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
            request.Location);
    }





    public static UpdateBatchCommand ToCommand(
        this UpdateBatchRequest request,
        Guid batchId)
    {
        return new UpdateBatchCommand(
            batchId,
            request.Quantity,
            request.ExpiryDate);
    }






    // =========================
    // DTO -> Response
    // =========================



    //=======PRODUCT=======


    public static ProductDetailResponse ToResponse(
        this ProductDto dto)
    {
        return new ProductDetailResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Category = dto.CategoryId.HasValue
                ? new ProductCategoryRef
                {
                    Id = dto.CategoryId.Value,
                    Name = dto.CategoryName ?? string.Empty
                }
                : null,
            Gtin = dto.Gtin,
            Unit = dto.UnitName,
            UnitId = dto.UnitId,
            OrganizationId = dto.OrganizationId,
            OrganizationName = dto.OrganizationName,
            Status = dto.Status
        };
    }

    public static ProductListItemResponse ToListItemResponse(
        this ProductDto dto)
    {
        return new ProductListItemResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            Gtin = dto.Gtin,
            Unit = dto.UnitName,
            UnitId = dto.UnitId,
            OrganizationId = dto.OrganizationId,
            OrganizationName = dto.OrganizationName,
            Status = dto.Status
        };
    }






    //=======ORGANIZATION========


    /// <summary>
    /// Maps an <see cref="OrganizationDto"/> to the swagger <c>OrganizationDetail</c> response shape.
    /// The organization "type" string is derived from the configured organization type code.
    /// </summary>
    public static OrganizationDetailResponse ToResponse(
        this OrganizationDto dto)
    {
        return new OrganizationDetailResponse
        {
            OrganizationId = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            OrganizationTypeId = dto.OrganizationTypeId,
            Type = dto.OrganizationTypeCode,
            Status = dto.Status.ToString()
        };
    }






    //=======BATCH========


    public static BatchDetailResponse ToResponse(
        this BatchDto dto)
    {
        return new BatchDetailResponse
        {
            BatchId = dto.Id,
            ProductId = dto.ProductId,
            ProductName = dto.ProductName,
            ProductGtin = dto.ProductGtin,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            BatchCode = dto.BatchCode,
            Quantity = dto.Quantity,
            RemainingQuantity = dto.RemainingQuantity,
            UnitId = dto.UnitId,
            UnitCode = dto.UnitCode,
            ProductionDate = DateOnly.FromDateTime(dto.ProductionDate),
            ExpiryDate = dto.ExpiryDate.HasValue ? DateOnly.FromDateTime(dto.ExpiryDate.Value) : null,
            Status = (int)dto.Status,
            CurrentOrganizationId = dto.CurrentOrganizationId,
            OrganizationName = dto.OrganizationName,
            Location = dto.Location,
            ProductionArea = dto.ProductionArea,

            CreatedAt = dto.CreatedAt,
            QrCodeUrl = dto.QrCodeUrl
        };
    }

    public static BatchListItemResponse ToListItemResponse(
        this BatchDto dto)
    {
        return new BatchListItemResponse
        {
            BatchId = dto.Id,
            ProductId = dto.ProductId,
            ProductName = dto.ProductName,
            ProductGtin = dto.ProductGtin,
            BatchCode = dto.BatchCode,
            Quantity = dto.Quantity,
            RemainingQuantity = dto.RemainingQuantity,
            UnitId = dto.UnitId,
            UnitCode = dto.UnitCode,
            Status = (int)dto.Status,
            StatusName = dto.Status.ToString(),
            CurrentOrganizationId = dto.CurrentOrganizationId,
            QrCodeUrl = dto.QrCodeUrl,
            CreatedAt = dto.CreatedAt
        };
    }


}
