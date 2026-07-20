using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Events;

/// <summary>
/// Detailed event response. Matches swagger <c>EventDetail</c>.
/// </summary>
public class EventDetail
{
    [JsonPropertyName("eventId")]
    public Guid EventId { get; set; }

    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("eventTypeId")]
    public Guid EventTypeId { get; set; }

    [JsonPropertyName("eventTypeCode")]
    public string? EventTypeCode { get; set; }

    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("performedByUserId")]
    public Guid PerformedByUserId { get; set; }

    [JsonPropertyName("eventData")]
    public string? EventData { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("previousHash")]
    public string? PreviousHash { get; set; }

    [JsonPropertyName("currentHash")]
    public string? CurrentHash { get; set; }

    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; }
}

/// <summary>
/// List item event response. Matches swagger <c>EventListItem</c> (allOf EventDetail).
/// </summary>
public class EventListItem : EventDetail
{
}

/// <summary>
/// Paged response for events. Matches swagger <c>EventPagedResponse</c>.
/// </summary>
public class EventPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<EventListItem> Items { get; set; } = new();

    public EventPagedResponse()
    {
    }

    public EventPagedResponse(IEnumerable<EventListItem> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}

/// <summary>
/// Created event data. Matches swagger <c>EventCreatedData</c>.
/// </summary>
public class EventCreatedData
{
    [JsonPropertyName("eventId")]
    public Guid EventId { get; set; }

    [JsonPropertyName("previousHash")]
    public string? PreviousHash { get; set; }

    [JsonPropertyName("currentHash")]
    public string? CurrentHash { get; set; }
}

/// <summary>
/// Hash chain verification response. Matches swagger verify schema { isValid, totalEvents }.
/// </summary>
public class HashChainVerifyResponse
{
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("totalEvents")]
    public int TotalEvents { get; set; }
}
