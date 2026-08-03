using AgriTrace.Domain.Enums;

namespace AgriTrace.Application.Contracts;

public class EventRequestDto
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchCode { get; set; }
    public Guid EventTypeId { get; set; }
    public string? EventTypeCode { get; set; }
    public string? EventTypeName { get; set; }
    public Guid OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string? RequestedByUserName { get; set; }
    public string? EventData { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public EventRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}
