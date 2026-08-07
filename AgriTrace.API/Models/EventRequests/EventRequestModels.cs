using System.ComponentModel.DataAnnotations;
using AgriTrace.Domain.Enums;

namespace AgriTrace.API.Models.EventRequests;

public class CreateEventRequestModel
{
    [Required]
    public Guid EventTypeId { get; set; }

    public Guid? BatchId { get; set; }

    public Guid? TargetOrganizationId { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class RejectEventRequestModel
{
    [StringLength(1000)]
    public string? Reason { get; set; }
}
