using AgriTrace.Domain.Enums;

namespace AgriTrace.Infrastructure.Sqlserver.Models;

public class EventRequestDataModel : BaseDataModel
{
    public Guid BatchId { get; set; }
    public Guid EventTypeId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string? EventData { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public EventRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }

    // Navigation
    public BatchDataModel Batch { get; set; } = null!;
    public EventTypeDataModel EventType { get; set; } = null!;
    public OrganizationDataModel Organization { get; set; } = null!;
    public UserDataModel RequestedByUser { get; set; } = null!;
    public UserDataModel? ReviewedByUser { get; set; }
}
