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
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Services;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Domain.Services;

public class EventServiceTests
{
    // ── Setup ────────────────────────────────────────────────────────────────
    private readonly Mock<IEventRepository> _repoMock = new();
    private readonly Mock<IHashChainService> _hashMock = new();
    private readonly EventService _sut;

    private static readonly Guid BatchId = Guid.NewGuid();
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid EventTypeId = Guid.NewGuid();

    public EventServiceTests()
    {
        _sut = new EventService(_repoMock.Object, _hashMock.Object);
    }

    private static SupplyChainEvent CreateEvent(string? eventData = "data", string? currentHash = null)
    {
        var evt = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, eventData, null, null, null);
        if (currentHash != null)
            evt.SetHash("GENESIS", currentHash);
        return evt;
    }

    // ── CreateEventAsync ─────────────────────────────────────────────────────
    [Fact]
    public async Task CreateEventAsync_NoExistingEvents_UsesPreviousHashGenesis()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync((SupplyChainEvent?)null);

        _hashMock
            .Setup(h => h.ComputeHash("GENESIS", "harvest"))
            .Returns("COMPUTED_HASH");

        var entity = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, "harvest", null, null, null);

        _repoMock
            .Setup(r => r.AddAsync(entity, default))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.CreateEventAsync(entity);

        // Assert
        _hashMock.Verify(h => h.ComputeHash("GENESIS", "harvest"), Times.Once);
        entity.PreviousHash.Should().Be("GENESIS");
        entity.CurrentHash.Should().Be("COMPUTED_HASH");
    }

    [Fact]
    public async Task CreateEventAsync_HasExistingEvent_UsesPreviousHashFromLastEvent()
    {
        // Arrange
        var lastEvent = CreateEvent(currentHash: "LAST_HASH");

        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync(lastEvent);

        _hashMock
            .Setup(h => h.ComputeHash("LAST_HASH", "new data"))
            .Returns("NEW_HASH");

        var newEntity = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, "new data", null, null, null);

        _repoMock
            .Setup(r => r.AddAsync(newEntity, default))
            .ReturnsAsync(newEntity);

        // Act
        await _sut.CreateEventAsync(newEntity);

        // Assert
        _hashMock.Verify(h => h.ComputeHash("LAST_HASH", "new data"), Times.Once);
        newEntity.PreviousHash.Should().Be("LAST_HASH");
        newEntity.CurrentHash.Should().Be("NEW_HASH");
    }

    [Fact]
    public async Task CreateEventAsync_CallsAddAsync()
    {
        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync((SupplyChainEvent?)null);

        _hashMock
            .Setup(h => h.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("HASH");

        var entity = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, "d", null, null, null);

        _repoMock
            .Setup(r => r.AddAsync(entity, default))
            .ReturnsAsync(entity);

        await _sut.CreateEventAsync(entity);

        _repoMock.Verify(r => r.AddAsync(entity, default), Times.Once);
    }

    // ── VerifyHashChainAsync ─────────────────────────────────────────────────
    [Fact]
    public async Task VerifyHashChainAsync_EmptyChain_ReturnsTrue()
    {
        _repoMock
            .Setup(r => r.GetByBatchAsync(BatchId, default))
            .ReturnsAsync(new List<SupplyChainEvent>().AsReadOnly());

        var result = await _sut.VerifyHashChainAsync(BatchId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyHashChainAsync_ValidChain_ReturnsTrue()
    {
        // Build a real chain of 2 events
        var hashService = new HashChainService();

        var data1 = "event one";
        var data2 = "event two";

        var hash1 = hashService.ComputeHash("GENESIS", data1);
        var hash2 = hashService.ComputeHash(hash1, data2);

        var evt1 = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, data1, null, null, null);
        evt1.SetHash("GENESIS", hash1);

        var evt2 = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, data2, null, null, null);
        evt2.SetHash(hash1, hash2);

        _repoMock
            .Setup(r => r.GetByBatchAsync(BatchId, default))
            .ReturnsAsync(new List<SupplyChainEvent> { evt1, evt2 }.AsReadOnly());

        // Use real hash service for verification
        var realSut = new EventService(_repoMock.Object, hashService);
        var result = await realSut.VerifyHashChainAsync(BatchId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyHashChainAsync_TamperedHash_ReturnsFalse()
    {
        var hashService = new HashChainService();

        var data1 = "event one";
        var hash1 = hashService.ComputeHash("GENESIS", data1);

        var evt1 = new SupplyChainEvent(BatchId, EventTypeId, OrgId, UserId, data1, null, null, null);
        // Tamper: set a wrong CurrentHash
        evt1.SetHash("GENESIS", "TAMPERED_HASH_NOT_VALID");

        _repoMock
            .Setup(r => r.GetByBatchAsync(BatchId, default))
            .ReturnsAsync(new List<SupplyChainEvent> { evt1 }.AsReadOnly());

        var realSut = new EventService(_repoMock.Object, hashService);
        var result = await realSut.VerifyHashChainAsync(BatchId);

        result.Should().BeFalse();
    }

    // ── GetByBatchAsync ───────────────────────────────────────────────────────
    [Fact]
    public async Task GetByBatchAsync_DelegatesToRepository()
    {
        var events = new List<SupplyChainEvent>().AsReadOnly();
        _repoMock
            .Setup(r => r.GetByBatchAsync(BatchId, default))
            .ReturnsAsync(events);

        var result = await _sut.GetByBatchAsync(BatchId);

        result.Should().BeSameAs(events);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────
    [Fact]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync((SupplyChainEvent?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }
}

