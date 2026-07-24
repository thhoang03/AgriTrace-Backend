namespace AgriTrace.Application.Contracts;

public sealed class SupplyChainEventDto
{
    public Guid Id { get; init; }

    public Guid BatchId { get; init; }

    public Guid EventTypeId { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid PerformedByUserId { get; init; }

    public string? EventData { get; init; }

    public string? Location { get; init; }

    public string? PreviousHash { get; init; }

    public string? CurrentHash { get; init; }

    public DateTime EventTime { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
