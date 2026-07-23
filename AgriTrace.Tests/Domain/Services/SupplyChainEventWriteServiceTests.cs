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

public class SupplyChainEventWriteServiceTests
{
    private readonly Mock<ISupplyChainEventWriteRepository> _repoMock = new();
    private readonly Mock<IHashChainService> _hashMock = new();
    private readonly SupplyChainEventWriteService _sut;

    private static readonly Guid BatchId = Guid.NewGuid();
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid EventTypeId = Guid.NewGuid();

    public SupplyChainEventWriteServiceTests()
    {
        _sut = new SupplyChainEventWriteService(_repoMock.Object, _hashMock.Object);
    }

    private SupplyChainEvent BuildEntity(string? data = "data")
        => new(BatchId, EventTypeId, OrgId, UserId, data, null, null, null);

    // ── CreateAsync — no prior event ─────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_NoPriorEvent_UsesPreviousHashGenesis()
    {
        var entity = BuildEntity("harvest");

        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync((SupplyChainEvent?)null);

        _hashMock
            .Setup(h => h.ComputeHash("GENESIS", "harvest"))
            .Returns("HASH_001");

        _repoMock
            .Setup(r => r.AddAsync(entity, default))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _hashMock.Verify(h => h.ComputeHash("GENESIS", "harvest"), Times.Once);
        entity.PreviousHash.Should().Be("GENESIS");
        entity.CurrentHash.Should().Be("HASH_001");
    }

    // ── CreateAsync — has prior event ────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_HasPriorEvent_UsesPriorCurrentHash()
    {
        var entity = BuildEntity("processing");

        var priorEvent = BuildEntity("harvest");
        priorEvent.SetHash("GENESIS", "PRIOR_HASH");

        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync(priorEvent);

        _hashMock
            .Setup(h => h.ComputeHash("PRIOR_HASH", "processing"))
            .Returns("CURR_HASH");

        _repoMock
            .Setup(r => r.AddAsync(entity, default))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _hashMock.Verify(h => h.ComputeHash("PRIOR_HASH", "processing"), Times.Once);
        entity.PreviousHash.Should().Be("PRIOR_HASH");
        entity.CurrentHash.Should().Be("CURR_HASH");
    }

    // ── CreateAsync — AddAsync is called ────────────────────────────────────
    [Fact]
    public async Task CreateAsync_CallsAddAsync()
    {
        var entity = BuildEntity();

        _repoMock
            .Setup(r => r.GetLastEventByBatchAsync(BatchId, default))
            .ReturnsAsync((SupplyChainEvent?)null);

        _hashMock
            .Setup(h => h.ComputeHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("HASH");

        _repoMock
            .Setup(r => r.AddAsync(entity, default))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _repoMock.Verify(r => r.AddAsync(entity, default), Times.Once);
    }

    // ── VerifyHashChainAsync — not supported ─────────────────────────────────
    [Fact]
    public async Task VerifyHashChainAsync_ThrowsNotSupportedException()
    {
        var act = () => _sut.VerifyHashChainAsync(BatchId);
        await act.Should().ThrowAsync<NotSupportedException>();
    }

    // ── UpdateAsync — delegates ──────────────────────────────────────────────
    [Fact]
    public async Task UpdateAsync_DelegatesToRepository()
    {
        var entity = BuildEntity();
        _repoMock.Setup(r => r.UpdateAsync(entity, default)).Returns(Task.CompletedTask);

        await _sut.UpdateAsync(entity);

        _repoMock.Verify(r => r.UpdateAsync(entity, default), Times.Once);
    }

    // ── DeleteAsync — delegates ──────────────────────────────────────────────
    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id, default)).Returns(Task.CompletedTask);

        await _sut.DeleteAsync(id);

        _repoMock.Verify(r => r.DeleteAsync(id, default), Times.Once);
    }
}

