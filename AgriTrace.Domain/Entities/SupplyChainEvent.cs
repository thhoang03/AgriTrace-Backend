using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class SupplyChainEvent : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid EventTypeId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid PerformedByUserId { get; private set; }

    public string? EventData { get; private set; }

    public string? Location { get; private set; }

    public string? PreviousHash { get; private set; }

    public string? CurrentHash { get; private set; }

    public DateTime EventTime { get; private set; }

    public Batch Batch { get; private set; }

    public EventType EventType { get; private set; }

    public Organization Organization { get; private set; }

    public User PerformedByUser { get; private set; }

    private SupplyChainEvent()
    {

    }

    public SupplyChainEvent(
        Guid batchId,
        Guid eventTypeId,
        Guid organizationId,
        Guid performedByUserId,
        string? eventData,
        string? location,
        string? previousHash,
        string? currentHash)
    {
        if (batchId == Guid.Empty)
            throw new ArgumentException(nameof(batchId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException(nameof(organizationId));

        if (performedByUserId == Guid.Empty)
            throw new ArgumentException(nameof(performedByUserId));

        BatchId = batchId;
        EventTypeId = eventTypeId;
        OrganizationId = organizationId;
        PerformedByUserId = performedByUserId;
        EventData = eventData;
        Location = location;
        PreviousHash = previousHash;
        CurrentHash = currentHash;
        EventTime = DateTime.UtcNow;
    }
    public void SetHash(
        string previousHash,
        string currentHash)
    {
        PreviousHash = previousHash;

        CurrentHash = currentHash;
    }
}