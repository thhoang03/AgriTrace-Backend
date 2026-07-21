using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Notifications;

/// <summary>
/// Notification response item. Matches swagger <c>NotificationItem</c>.
/// </summary>
public class NotificationItem
{
    [JsonPropertyName("notificationId")]
    public Guid NotificationId { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("isRead")]
    public bool IsRead { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paged response for notifications. Matches swagger <c>NotificationPagedResponse</c>.
/// </summary>
public class NotificationPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<NotificationItem> Items { get; set; } = new();

    public NotificationPagedResponse()
    {
    }

    public NotificationPagedResponse(IEnumerable<NotificationItem> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
