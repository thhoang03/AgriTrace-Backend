using AgriTrace.Application.Features.SupplyChainEvents.Commands;
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
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.SupplyChainEvents;

public class CreateSupplyChainEventCommandTests
{
    private readonly Mock<ISupplyChainEventWriteService> _writeMock = new();
    private readonly CreateSupplyChainEventCommandHandler _sut;

    private static readonly Guid BatchId = Guid.NewGuid();
    private static readonly Guid EventTypeId = Guid.NewGuid();
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    public CreateSupplyChainEventCommandTests()
    {
        _sut = new CreateSupplyChainEventCommandHandler(_writeMock.Object);
    }

    // ── Happy path ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var command = new CreateSupplyChainEventCommand(
            BatchId, EventTypeId, OrgId, UserId, "harvest data", "Ha Noi");

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<SupplyChainEvent>(), default))
            .ReturnsAsync((SupplyChainEvent e, CancellationToken _) => e);

        await _sut.Handle(command, default);

        _writeMock.Verify(s => s.CreateAsync(It.IsAny<SupplyChainEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCorrectDto()
    {
        var command = new CreateSupplyChainEventCommand(
            BatchId, EventTypeId, OrgId, UserId, "harvest data", "Ha Noi");

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<SupplyChainEvent>(), default))
            .ReturnsAsync((SupplyChainEvent e, CancellationToken _) => e);

        var result = await _sut.Handle(command, default);

        result.BatchId.Should().Be(BatchId);
        result.EventTypeId.Should().Be(EventTypeId);
        result.OrganizationId.Should().Be(OrgId);
        result.PerformedByUserId.Should().Be(UserId);
        result.EventData.Should().Be("harvest data");
        result.Location.Should().Be("Ha Noi");
    }

    [Fact]
    public async Task Handle_NullEventData_DoesNotThrow()
    {
        var command = new CreateSupplyChainEventCommand(
            BatchId, EventTypeId, OrgId, UserId, null, null);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<SupplyChainEvent>(), default))
            .ReturnsAsync((SupplyChainEvent e, CancellationToken _) => e);

        var act = () => _sut.Handle(command, default);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ValidCommand_EntityPassedToServiceHasCorrectBatchId()
    {
        var command = new CreateSupplyChainEventCommand(
            BatchId, EventTypeId, OrgId, UserId, "data", null);

        SupplyChainEvent? captured = null;
        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<SupplyChainEvent>(), default))
            .Callback<SupplyChainEvent, CancellationToken>((e, _) => captured = e)
            .ReturnsAsync((SupplyChainEvent e, CancellationToken _) => e);

        await _sut.Handle(command, default);

        captured.Should().NotBeNull();
        captured!.BatchId.Should().Be(BatchId);
        captured.OrganizationId.Should().Be(OrgId);
        captured.PerformedByUserId.Should().Be(UserId);
    }
}

