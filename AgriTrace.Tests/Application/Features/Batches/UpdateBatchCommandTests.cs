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
/// NOTE: Handlers call batch.Adapt&lt;BatchDto&gt;() via Mapster at the end.
/// Mapster.dll is blocked by Windows Application Control policy on this machine.
/// Tests verify entity state and handler behavior BEFORE the Adapt step,
/// catching FileLoadException where it occurs at the return statement.
/// </summary>
public class UpdateBatchCommandTests
{
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 1, 1);
    private static readonly DateTime Expiry = new(2025, 12, 31);

    private Batch BuildBatch(decimal qty = 100m)
        => new(ProductId, "ORIG001", qty, UnitId, Production, Expiry);

    private static UpdateBatchCommandHandler Build(
        Mock<IBatchReadService>? readMock = null,
        Mock<IBatchWriteService>? writeMock = null)
    {
        readMock ??= new Mock<IBatchReadService>();
        writeMock ??= new Mock<IBatchWriteService>();
        return new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);
    }

    // ── Not found ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_BatchNotFound_ThrowsNotFoundException()
    {
        var readMock = new Mock<IBatchReadService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var sut = Build(readMock);
        var act = () => sut.Handle(new UpdateBatchCommand(Guid.NewGuid(), 200m, null), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Null quantity — keep original ─────────────────────────────────────────
    [Fact]
    public async Task Handle_NullQuantity_EntityRetainsOriginalQuantity()
    {
        var batch = BuildBatch(100m);
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(new UpdateBatchCommand(batch.Id, null, null), default); }
        catch (System.IO.FileLoadException) { }

        batch.Quantity.Should().Be(100m);
    }

    // ── Explicit quantity — updates ───────────────────────────────────────────
    [Fact]
    public async Task Handle_ExplicitQuantity_EntityQuantityIsUpdated()
    {
        var batch = BuildBatch(100m);
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(new UpdateBatchCommand(batch.Id, 250m, null), default); }
        catch (System.IO.FileLoadException) { }

        batch.Quantity.Should().Be(250m);
    }

    // ── Null expiry — keep original ───────────────────────────────────────────
    [Fact]
    public async Task Handle_NullExpiryDate_EntityRetainsOriginalExpiry()
    {
        var batch = BuildBatch(100m);
        var originalExpiry = batch.ExpiryDate;
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(new UpdateBatchCommand(batch.Id, null, null), default); }
        catch (System.IO.FileLoadException) { }

        batch.ExpiryDate.Should().Be(originalExpiry);
    }

    // ── Explicit expiry — updates ─────────────────────────────────────────────
    [Fact]
    public async Task Handle_ExplicitExpiryDate_EntityExpiryIsUpdated()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);
        var newExpiry = new DateOnly(2027, 6, 15);

        try { await sut.Handle(new UpdateBatchCommand(batch.Id, null, newExpiry), default); }
        catch (System.IO.FileLoadException) { }

        batch.ExpiryDate.Should().Be(newExpiry.ToDateTime(TimeOnly.MinValue));
    }

    // ── UpdateAsync is called ─────────────────────────────────────────────────
    [Fact]
    public async Task Handle_Valid_CallsUpdateAsync()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(new UpdateBatchCommand(batch.Id, 150m, null), default); }
        catch (System.IO.FileLoadException) { }

        writeMock.Verify(s => s.UpdateAsync(batch, default), Times.Once);
    }
}

