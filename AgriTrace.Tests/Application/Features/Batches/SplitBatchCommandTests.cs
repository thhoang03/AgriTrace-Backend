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
using AgriTrace.Domain.Interfaces.Outbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Batches;

public class SplitBatchCommandTests
{
    private readonly Mock<IBatchReadService> _readMock = new();
    private readonly Mock<IBatchWriteService> _writeMock = new();
    private readonly Mock<IBatchSplitRepository> _splitRepoMock = new();
    private readonly SplitBatchCommandHandler _sut;

    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 1, 1);

    public SplitBatchCommandTests()
    {
        _sut = new SplitBatchCommandHandler(
            _readMock.Object,
            _writeMock.Object,
            _splitRepoMock.Object);
    }

    private Batch BuildBatch(decimal qty = 200m)
        => new(ProductId, "PARENT01", qty, UnitId, Production, null);

    private static List<SplitDetailInput> TwoSplits(decimal q1 = 80m, decimal q2 = 60m)
        => new() { new(q1, UnitId), new(q2, UnitId) };

    // ── Validation ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_NullSplits_ThrowsArgumentException()
    {
        var command = new SplitBatchCommand(Guid.NewGuid(), null!, UserId);
        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_OneSplit_ThrowsArgumentException()
    {
        var command = new SplitBatchCommand(
            Guid.NewGuid(),
            new List<SplitDetailInput> { new(50m, UnitId) },
            UserId);

        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*at least two*");
    }

    [Fact]
    public async Task Handle_BatchNotFound_ThrowsNotFoundException()
    {
        _readMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var command = new SplitBatchCommand(Guid.NewGuid(), TwoSplits(), UserId);
        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Happy path ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_TwoSplits_ReturnsResultWithParentId()
    {
        var parent = BuildBatch(200m);

        _readMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(parent);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _writeMock
            .Setup(s => s.UpdateAsync(parent, default))
            .Returns(Task.CompletedTask);

        _splitRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchSplit>(), default))
            .ReturnsAsync((BatchSplit s, CancellationToken _) => s);

        var command = new SplitBatchCommand(parent.Id, TwoSplits(80m, 60m), UserId);
        var result = await _sut.Handle(command, default);

        result.ParentBatchId.Should().Be(parent.Id);
        result.ChildBatchIds.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_TwoSplits_ReducesParentRemainingQuantity()
    {
        var parent = BuildBatch(200m);

        _readMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(parent);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _writeMock
            .Setup(s => s.UpdateAsync(parent, default))
            .Returns(Task.CompletedTask);

        _splitRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchSplit>(), default))
            .ReturnsAsync((BatchSplit s, CancellationToken _) => s);

        var command = new SplitBatchCommand(parent.Id, TwoSplits(80m, 60m), UserId);
        await _sut.Handle(command, default);

        parent.RemainingQuantity.Should().Be(60m); // 200 - 80 - 60
    }

    [Fact]
    public async Task Handle_Valid_PersistsSplitRecord()
    {
        var parent = BuildBatch(200m);

        _readMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(parent);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _writeMock
            .Setup(s => s.UpdateAsync(parent, default))
            .Returns(Task.CompletedTask);

        _splitRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchSplit>(), default))
            .ReturnsAsync((BatchSplit s, CancellationToken _) => s);

        await _sut.Handle(new SplitBatchCommand(parent.Id, TwoSplits(), UserId), default);

        _splitRepoMock.Verify(r => r.AddAsync(It.IsAny<BatchSplit>(), default), Times.Once);
        _writeMock.Verify(s => s.UpdateAsync(parent, default), Times.Once);
    }
}

