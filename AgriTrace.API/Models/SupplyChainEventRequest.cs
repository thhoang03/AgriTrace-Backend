using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for recording a new supply chain event on a batch.
/// </summary>
public class CreateSupplyChainEventRequest
{
    /// <summary>
    /// The Id of the EventType (e.g. Harvest, Transport, Storage).
    /// </summary>
    [Required]
    public Guid EventTypeId { get; set; }

    /// <summary>
    /// The Id of the Organization performing this event.
    /// </summary>
    [Required]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// The Id of the User who performed this event.
    /// </summary>
    [Required]
    public Guid PerformedByUserId { get; set; }

    /// <summary>
    /// JSON or plain-text payload describing the event details.
    /// </summary>
    [StringLength(4000)]
    public string? EventData { get; set; }

    /// <summary>
    /// GPS coordinates or location name where the event took place.
    /// </summary>
    [StringLength(500)]
    public string? Location { get; set; }
}
