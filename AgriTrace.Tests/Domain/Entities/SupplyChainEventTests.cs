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
using FluentAssertions;

namespace AgriTrace.Tests.Domain.Entities;

public class SupplyChainEventTests
{
    private static readonly Guid ValidBatchId = Guid.NewGuid();
    private static readonly Guid ValidEventTypeId = Guid.NewGuid();
    private static readonly Guid ValidOrgId = Guid.NewGuid();
    private static readonly Guid ValidUserId = Guid.NewGuid();

    private static SupplyChainEvent CreateValid(
        string? eventData = "some data",
        string? location = "Ha Noi")
        => new(ValidBatchId, ValidEventTypeId, ValidOrgId, ValidUserId, eventData, location, null, null);

    // ── Constructor — happy path ─────────────────────────────────────────────
    [Fact]
    public void Constructor_ValidParams_SetsAllProperties()
    {
        var evt = CreateValid("harvest data", "Hanoi");

        evt.BatchId.Should().Be(ValidBatchId);
        evt.EventTypeId.Should().Be(ValidEventTypeId);
        evt.OrganizationId.Should().Be(ValidOrgId);
        evt.PerformedByUserId.Should().Be(ValidUserId);
        evt.EventData.Should().Be("harvest data");
        evt.Location.Should().Be("Hanoi");
        evt.PreviousHash.Should().BeNull();
        evt.CurrentHash.Should().BeNull();
    }

    [Fact]
    public void Constructor_SetsEventTimeToUtcNow()
    {
        var before = DateTime.UtcNow;
        var evt = CreateValid();
        var after = DateTime.UtcNow;

        evt.EventTime.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Constructor_NullEventData_IsAllowed()
    {
        var act = () => CreateValid(eventData: null);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_NullLocation_IsAllowed()
    {
        var act = () => CreateValid(location: null);
        act.Should().NotThrow();
    }

    // ── Constructor — guard clauses ──────────────────────────────────────────
    [Fact]
    public void Constructor_EmptyBatchId_Throws()
    {
        var act = () => new SupplyChainEvent(
            Guid.Empty, ValidEventTypeId, ValidOrgId, ValidUserId, null, null, null, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyOrganizationId_Throws()
    {
        var act = () => new SupplyChainEvent(
            ValidBatchId, ValidEventTypeId, Guid.Empty, ValidUserId, null, null, null, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyPerformedByUserId_Throws()
    {
        var act = () => new SupplyChainEvent(
            ValidBatchId, ValidEventTypeId, ValidOrgId, Guid.Empty, null, null, null, null);
        act.Should().Throw<ArgumentException>();
    }

    // ── SetHash ──────────────────────────────────────────────────────────────
    [Fact]
    public void SetHash_UpdatesBothHashes()
    {
        var evt = CreateValid();
        evt.SetHash("GENESIS", "ABC123DEF");

        evt.PreviousHash.Should().Be("GENESIS");
        evt.CurrentHash.Should().Be("ABC123DEF");
    }

    [Fact]
    public void SetHash_CalledTwice_OverwritesHashes()
    {
        var evt = CreateValid();
        evt.SetHash("GENESIS", "FIRST_HASH");
        evt.SetHash("FIRST_HASH", "SECOND_HASH");

        evt.PreviousHash.Should().Be("FIRST_HASH");
        evt.CurrentHash.Should().Be("SECOND_HASH");
    }
}

