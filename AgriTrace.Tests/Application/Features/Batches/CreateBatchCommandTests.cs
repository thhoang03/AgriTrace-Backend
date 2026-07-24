using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Batches.Commands;
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

namespace AgriTrace.Tests.Application.Features.Batches;

/// <summary>
/// Tests for CreateBatchCommandHandler.
/// NOTE: Handlers use Mapster's batch.Adapt&lt;BatchDto&gt;() at the end.
/// Mapster is blocked by Application Control policy on this machine, so tests
/// that directly call Handle() will fail at the Adapt step.
/// Strategy: verify that CreateAsync receives an entity with the correct values
/// (Callback inspection) WITHOUT calling Handle() all the way through Mapster.
/// For tests that need the full result, we verify the entity state instead.
/// </summary>
public class CreateBatchCommandTests
{
    private readonly Mock<IBatchWriteService> _writeMock = new();

    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 3, 1);
    private static readonly DateTime Expiry = new(2026, 3, 1);

    // ── Verify entity created with correct ProductId and Quantity ─────────────
    [Fact]
    public async Task Handle_ValidCommand_CreatesEntityWithCorrectProductId()
    {
        var command = new CreateBatchCommand(ProductId, UnitId, 100m, Production, Expiry);

        Batch? captured = null;
        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .Callback<Batch, CancellationToken>((b, _) => captured = b)
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        // Act — wrap in try/catch to ignore Mapster block at the end
        try { await new CreateBatchCommandHandler(_writeMock.Object).Handle(command, default); }
        catch (System.IO.FileLoadException) { /* Mapster.dll blocked by OS policy — expected */ }

        captured.Should().NotBeNull();
        captured!.ProductId.Should().Be(ProductId);
        captured.UnitId.Should().Be(UnitId);
        captured.Quantity.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_ValidCommand_BatchCodeIsEightCharsUppercaseHex()
    {
        var command = new CreateBatchCommand(ProductId, UnitId, 100m, Production, Expiry);

        string? capturedCode = null;
        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .Callback<Batch, CancellationToken>((b, _) => capturedCode = b.BatchCode)
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        try { await new CreateBatchCommandHandler(_writeMock.Object).Handle(command, default); }
        catch (System.IO.FileLoadException) { }

        capturedCode.Should().NotBeNull();
        capturedCode!.Length.Should().Be(8);
        capturedCode.Should().MatchRegex("^[0-9A-F]+$");
    }

    [Fact]
    public async Task Handle_ValidCommand_EntityStatusIsCreated()
    {
        var command = new CreateBatchCommand(ProductId, UnitId, 100m, Production, null);

        BatchStatus? capturedStatus = null;
        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .Callback<Batch, CancellationToken>((b, _) => capturedStatus = b.Status)
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        try { await new CreateBatchCommandHandler(_writeMock.Object).Handle(command, default); }
        catch (System.IO.FileLoadException) { }

        capturedStatus.Should().Be(BatchStatus.Created);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var command = new CreateBatchCommand(ProductId, UnitId, 100m, Production, Expiry);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        try { await new CreateBatchCommandHandler(_writeMock.Object).Handle(command, default); }
        catch (System.IO.FileLoadException) { }

        _writeMock.Verify(s => s.CreateAsync(It.IsAny<Batch>(), default), Times.Once);
    }
}

