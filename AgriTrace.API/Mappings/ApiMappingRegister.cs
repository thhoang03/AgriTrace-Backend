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

    public static CreateProductCommand ToCommand(
        this ProductRequest request)
    {
        // TODO Phase 8+: resolve UnitId (Guid) from request.Unit (string) via a unit lookup service.
        return new CreateProductCommand(
            request.OrganizationId,
            request.CategoryId,
            null,
            request.Name);
    }



    public static UpdateProductCommand ToCommand(
        this ProductRequest request,
        Guid id)
    {
        // TODO Phase 8+: resolve UnitId (Guid) from request.Unit (string) via a unit lookup service.
        return new UpdateProductCommand(
            id,
            request.CategoryId,
            null,
            request.Name);
    }





    //========ORGANIZATION=======


    public static CreateOrganizationCommand ToCommand(
        this OrganizationRequest request)
    {
        return new CreateOrganizationCommand(
            request.Type,
            request.Name,
            request.Address);
    }



    public static UpdateOrganizationCommand ToCommand(
        this OrganizationRequest request,
        Guid id)
    {
        return new UpdateOrganizationCommand(
            id,
            request.Type,
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
            request.ExpiryDate?.ToDateTime(TimeOnly.MinValue));
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
            Unit = dto.UnitName,
            OrganizationId = dto.OrganizationId,
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
            Unit = dto.UnitName,
            OrganizationId = dto.OrganizationId,
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
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            BatchCode = dto.BatchCode,
            Quantity = dto.Quantity,
            UnitId = dto.UnitId,
            UnitCode = dto.UnitCode,
            ProductionDate = DateOnly.FromDateTime(dto.ProductionDate),
            ExpiryDate = dto.ExpiryDate.HasValue ? DateOnly.FromDateTime(dto.ExpiryDate.Value) : null,
            Status = (int)dto.Status,
            CurrentOrganizationId = dto.CurrentOrganizationId,
            OrganizationName = dto.OrganizationName,
            QrCodeUrl = dto.QrCodeUrl,
            CreatedAt = dto.CreatedAt
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
            BatchCode = dto.BatchCode,
            Quantity = dto.Quantity,
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
