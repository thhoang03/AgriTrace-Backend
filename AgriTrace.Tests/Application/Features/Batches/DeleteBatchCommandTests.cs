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
/// Tests for DeleteBatchCommandHandler.
/// Handler: GetByIdAsync → NotFoundException if null → DeleteAsync.
/// </summary>
public class DeleteBatchCommandTests
{
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 1, 1);

    private static Batch BuildBatch()
        => new(ProductId, "BATCH001", 100m, UnitId, Production, null);

    // ── Not found ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_BatchNotFound_ThrowsNotFoundException()
    {
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var sut = new DeleteBatchCommandHandler(readMock.Object, writeMock.Object);
        var act = () => sut.Handle(new DeleteBatchCommand(Guid.NewGuid()), default);

        await act.Should().ThrowAsync<AgriTrace.Domain.Common.NotFoundException>();
    }

    [Fact]
    public async Task Handle_BatchNotFound_NeverCallsDeleteAsync()
    {
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var sut = new DeleteBatchCommandHandler(readMock.Object, writeMock.Object);

        try { await sut.Handle(new DeleteBatchCommand(Guid.NewGuid()), default); }
        catch (AgriTrace.Domain.Common.NotFoundException) { }

        writeMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    // ── Happy path ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidBatch_CallsDeleteAsync()
    {
        var batch = BuildBatch();
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(batch.Id, default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.DeleteAsync(batch.Id, default)).Returns(Task.CompletedTask);

        var sut = new DeleteBatchCommandHandler(readMock.Object, writeMock.Object);
        await sut.Handle(new DeleteBatchCommand(batch.Id), default);

        writeMock.Verify(s => s.DeleteAsync(batch.Id, default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidBatch_PassesCorrectIdToDeleteAsync()
    {
        var batch = BuildBatch();
        Guid? capturedId = null;
        var readMock = new Mock<IBatchReadService>();
        var writeMock = new Mock<IBatchWriteService>();
        readMock.Setup(s => s.GetByIdAsync(batch.Id, default)).ReturnsAsync(batch);
        writeMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), default))
            .Callback<Guid, CancellationToken>((id, _) => capturedId = id)
            .Returns(Task.CompletedTask);

        var sut = new DeleteBatchCommandHandler(readMock.Object, writeMock.Object);
        await sut.Handle(new DeleteBatchCommand(batch.Id), default);

        capturedId.Should().Be(batch.Id);
    }
}

