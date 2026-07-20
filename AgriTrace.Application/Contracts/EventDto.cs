namespace AgriTrace.Application.Contracts;

/// <summary>
/// DTO for a supply-chain event. Mirrors swagger <c>EventDetail</c>.
/// </summary>
public class EventDto
{
    public Guid EventId { get; set; }

    public Guid BatchId { get; set; }

    public Guid EventTypeId { get; set; }

    public string? EventTypeCode { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid PerformedByUserId { get; set; }

    public string? EventData { get; set; }

    public string? Location { get; set; }

    public string? PreviousHash { get; set; }

    public string? CurrentHash { get; set; }

    public DateTime EventTime { get; set; }
}
