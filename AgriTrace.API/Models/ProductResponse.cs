using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AgriTrace.Domain.Entities.Products;

namespace AgriTrace.API.Models;

/// <summary>
/// Detailed product response.
/// Exposes both <c>id</c> and <c>productId</c>, plus <c>unitId</c> for frontend compatibility.
/// </summary>
public class ProductDetailResponse
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("id")]
    public Guid ItemId => Id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("category")]
    public ProductCategoryRef? Category { get; set; }

    [JsonPropertyName("categoryId")]
    public Guid? CategoryId => Category?.Id;

    [JsonPropertyName("categoryName")]
    public string? CategoryName => Category?.Name;

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    [JsonPropertyName("unitCode")]
    public string? UnitCode => Unit;

    [JsonPropertyName("unitName")]
    public string? UnitName => Unit;

    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("status")]
    public ProductStatus Status { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive => Status != ProductStatus.Inactive;
}

/// <summary>
/// Category reference used by <see cref="ProductDetailResponse"/>.
/// </summary>
public class ProductCategoryRef
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

/// <summary>
/// List item product response.
/// Exposes both <c>id</c> and <c>productId</c>, plus <c>unitId</c>.
/// </summary>
public class ProductListItemResponse
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("id")]
    public Guid ItemId => Id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("categoryId")]
    public Guid? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    [JsonPropertyName("unitCode")]
    public string? UnitCode => Unit;

    [JsonPropertyName("unitName")]
    public string? UnitName => Unit;

    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("status")]
    public ProductStatus Status { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive => Status != ProductStatus.Inactive;
}

/// <summary>
/// Paged response for products.
/// </summary>
public class ProductPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<ProductListItemResponse> Items { get; set; } = new();

    public ProductPagedResponse()
    {
    }

    public ProductPagedResponse(IEnumerable<ProductListItemResponse> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}

/// <summary>
/// Created data for a product.
/// </summary>
public class ProductCreatedData
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("id")]
    public Guid ItemId => Id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
