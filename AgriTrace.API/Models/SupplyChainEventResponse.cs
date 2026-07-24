namespace AgriTrace.API.Models;

/// <summary>
/// Response payload for a supply chain event.
/// </summary>
public class SupplyChainEventResponse
{
    public Guid EventId { get; set; }

    public Guid BatchId { get; set; }

    public Guid EventTypeId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid PerformedByUserId { get; set; }

    public string? EventData { get; set; }

    public string? Location { get; set; }

    /// <summary>
    /// Hash of the previous event in the chain ("GENESIS" for the first event).
    /// </summary>
    public string? PreviousHash { get; set; }

    /// <summary>
    /// SHA-256 hash computed from PreviousHash + EventData.
    /// </summary>
    public string? CurrentHash { get; set; }

    public DateTime EventTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
