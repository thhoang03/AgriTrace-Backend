using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Users;

/// <summary>
/// Detailed user response. Matches swagger <c>UserDetail</c>.
/// ID Type Note: swagger declares userId/organizationId as integer, but the domain uses <c>Guid</c>
/// (Phase 1 deviation; annotation deferred to Phase 11).
/// </summary>
public class UserDetail
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("organizationId")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// List item user response. Matches swagger <c>UserListItem</c>.
/// </summary>
public class UserListItem
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("organizationId")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paged response for users. Matches swagger <c>UserPagedResponse</c> (PagedMeta + items).
/// </summary>
public class UserPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<UserListItem> Items { get; set; } = new();

    public UserPagedResponse()
    {
    }

    public UserPagedResponse(IEnumerable<UserListItem> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
