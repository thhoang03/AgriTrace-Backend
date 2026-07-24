using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;

namespace AgriTrace.Application.Features.Events.Queries;

/// <summary>
/// Maps a <see cref="SupplyChainEvent"/> domain entity to an <see cref="EventDto"/>.
/// </summary>
public static class EventMapper
{
    public static EventDto ToDto(SupplyChainEvent e, string? eventTypeCode) => new()
    {
        EventId = e.Id,
        BatchId = e.BatchId,
        EventTypeId = e.EventTypeId,
        EventTypeCode = eventTypeCode,
        OrganizationId = e.OrganizationId,
        PerformedByUserId = e.PerformedByUserId,
        EventData = e.EventData,
        Location = e.Location,
        PreviousHash = e.PreviousHash,
        CurrentHash = e.CurrentHash,
        EventTime = e.EventTime
    };
}

