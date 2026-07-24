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
/// Tests verify entity state mutation BEFORE the Adapt step.
/// </summary>
public class UpdateBatchStatusCommandTests
{
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 1, 1);

    private static Batch BuildBatch()
        => new(ProductId, "BATCH001", 100m, UnitId, Production, null);

    [Fact]
    public async Task Handle_BatchNotFound_ThrowsNotFoundException()
    {
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var sut = new UpdateBatchStatusCommandHandler(readMock.Object, writeMock.Object);
        var act = () => sut.Handle(new UpdateBatchStatusCommand(Guid.NewGuid(), 1), default);

        await act.Should().ThrowAsync<AgriTrace.Domain.Common.NotFoundException>();
    }

    [Fact]
    public async Task Handle_ValidStatus_EntityStatusIsChanged()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchStatusCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(
            new UpdateBatchStatusCommand(batch.Id, (int)BatchStatus.Transporting), default); }
        catch (System.IO.FileLoadException) { }

        batch.Status.Should().Be(BatchStatus.Transporting);
    }

    [Fact]
    public async Task Handle_ValidStatus_CallsUpdateAsync()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchStatusCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(
            new UpdateBatchStatusCommand(batch.Id, (int)BatchStatus.Distributed), default); }
        catch (System.IO.FileLoadException) { }

        writeMock.Verify(s => s.UpdateAsync(batch, default), Times.Once);
    }

    [Fact]
    public async Task Handle_RecalledStatus_EntityStatusIsRecalled()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.UpdateAsync(batch, default)).Returns(Task.CompletedTask);

        var sut = new UpdateBatchStatusCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(
            new UpdateBatchStatusCommand(batch.Id, (int)BatchStatus.Recalled), default); }
        catch (System.IO.FileLoadException) { }

        batch.Status.Should().Be(BatchStatus.Recalled);
    }
}

