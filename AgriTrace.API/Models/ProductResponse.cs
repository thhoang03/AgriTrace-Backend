using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Detailed product response. Matches swagger <c>ProductDetail</c>.
/// </summary>
/// <remarks>
/// ID Type Note: swagger declares product/category/organization ids as integer, but the domain uses <c>Guid</c>.
/// Per the documented Phase 1 decision the API keeps <c>Guid</c> (annotation deferred to Phase 11).
/// <c>isActive</c> is sourced from the product response DTO; the domain <c>Product</c> entity does not yet
/// carry an IsActive flag, so it defaults to true until a domain field is introduced (Phase 11 follow-up).
/// </remarks>
public class ProductDetailResponse
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("category")]
    public ProductCategoryRef? Category { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("status")]
    public ProductStatus Status { get; set; }
}

/// <summary>
/// Category reference used by <see cref="ProductDetailResponse"/>. Matches swagger <c>ProductDetail.category</c>.
/// </summary>
public class ProductCategoryRef
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

/// <summary>
/// List item product response. Matches swagger <c>ProductListItem</c>.
/// </summary>
public class ProductListItemResponse
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("categoryId")]
    public Guid? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("status")]
    public ProductStatus Status { get; set; }
}

/// <summary>
/// Paged response for products. Matches swagger <c>ProductPagedResponse</c>.
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
/// Created data for a product. Matches swagger <c>ProductCreatedData</c>.
/// </summary>
public class ProductCreatedData
{
    [JsonPropertyName("productId")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
