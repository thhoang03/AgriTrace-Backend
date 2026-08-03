using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Enums;

namespace AgriTrace.Domain.Entities.Events;

public class EventRequest : BaseEntity
{
    public Guid BatchId { get; private set; }
    public Guid EventTypeId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public string? EventData { get; private set; }
    public string? Location { get; private set; }
    public string? Description { get; private set; }
    public EventRequestStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }

    public Batch Batch { get; private set; } = null!;
    public EventType EventType { get; private set; } = null!;
    public Organization Organization { get; private set; } = null!;
    public User RequestedByUser { get; private set; } = null!;
    public User? ReviewedByUser { get; private set; }

    private EventRequest() { }

    public EventRequest(
        Guid batchId,
        Guid eventTypeId,
        Guid organizationId,
        Guid requestedByUserId,
        string? eventData,
        string? location,
        string? description)
    {
        if (batchId == Guid.Empty)
            throw new ArgumentException("Batch ID is required", nameof(batchId));
        if (eventTypeId == Guid.Empty)
            throw new ArgumentException("Event Type ID is required", nameof(eventTypeId));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required", nameof(organizationId));
        if (requestedByUserId == Guid.Empty)
            throw new ArgumentException("Requested By User ID is required", nameof(requestedByUserId));

        Id = Guid.NewGuid();
        BatchId = batchId;
        EventTypeId = eventTypeId;
        OrganizationId = organizationId;
        RequestedByUserId = requestedByUserId;
        EventData = eventData;
        Location = location;
        Description = description;
        Status = EventRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static EventRequest Rehydrate(
        Guid id,
        Guid batchId,
        Guid eventTypeId,
        Guid organizationId,
        Guid requestedByUserId,
        string? eventData,
        string? location,
        string? description,
        EventRequestStatus status,
        string? rejectionReason,
        DateTime createdAt,
        DateTime? reviewedAt,
        Guid? reviewedByUserId,
        Batch? batch = null,
        EventType? eventType = null,
        Organization? organization = null,
        User? requestedByUser = null,
        User? reviewedByUser = null)
    {
        var entity = new EventRequest
        {
            Id = id,
            BatchId = batchId,
            EventTypeId = eventTypeId,
            OrganizationId = organizationId,
            RequestedByUserId = requestedByUserId,
            EventData = eventData,
            Location = location,
            Description = description,
            Status = status,
            RejectionReason = rejectionReason,
            CreatedAt = createdAt,
            ReviewedAt = reviewedAt,
            ReviewedByUserId = reviewedByUserId,
            Batch = batch!,
            EventType = eventType!,
            Organization = organization!,
            RequestedByUser = requestedByUser!,
            ReviewedByUser = reviewedByUser
        };

        return entity;
    }

    public void Approve(Guid reviewedByUserId)
    {
        if (Status != EventRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");

        Status = EventRequestStatus.Approved;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Reject(Guid reviewedByUserId, string? reason)
    {
        if (Status != EventRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        Status = EventRequestStatus.Rejected;
        RejectionReason = reason;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = DateTime.UtcNow;
    }
}
