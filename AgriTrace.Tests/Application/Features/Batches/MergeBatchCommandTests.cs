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

public class MergeBatchCommandTests
{
    private readonly Mock<IBatchReadService> _readMock = new();
    private readonly Mock<IBatchWriteService> _writeMock = new();
    private readonly Mock<IBatchMergeRepository> _mergeRepoMock = new();
    private readonly MergeBatchCommandHandler _sut;

    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTime Production = new(2025, 1, 1);
    private static readonly DateOnly MergeDate = new(2025, 6, 1);

    public MergeBatchCommandTests()
    {
        _sut = new MergeBatchCommandHandler(
            _readMock.Object,
            _writeMock.Object,
            _mergeRepoMock.Object);
    }

    private static Batch BuildBatch(decimal qty = 100m, Guid? orgId = null)
    {
        var b = new Batch(ProductId, Guid.NewGuid().ToString("N")[..8].ToUpper(), qty, UnitId, Production, null);
        if (orgId.HasValue) b.ChangeOrganization(orgId.Value);
        return b;
    }

    private MergeBatchCommand BuildCommand(List<Guid> sourceIds)
        => new(sourceIds, ProductId, 200m, UnitId, MergeDate);

    // ── Validation ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_NullSourceBatchIds_ThrowsArgumentException()
    {
        var command = new MergeBatchCommand(null!, ProductId, 200m, UnitId, MergeDate);
        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_OneSourceBatch_ThrowsArgumentException()
    {
        var command = new MergeBatchCommand(
            new List<Guid> { Guid.NewGuid() },
            ProductId, 200m, UnitId, MergeDate);

        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*at least two*");
    }

    // ── Not found ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_SourceBatchNotFound_ThrowsNotFoundException()
    {
        _readMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Batch?)null);

        var command = BuildCommand(new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });
        var act = () => _sut.Handle(command, default);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Happy path ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_TwoSources_ReturnsMergeBatchResult()
    {
        var orgId = Guid.NewGuid();
        var src1 = BuildBatch(100m, orgId);
        var src2 = BuildBatch(80m, orgId);

        var sourceIds = new List<Guid> { src1.Id, src2.Id };

        _readMock
            .Setup(s => s.GetByIdAsync(src1.Id, default)).ReturnsAsync(src1);
        _readMock
            .Setup(s => s.GetByIdAsync(src2.Id, default)).ReturnsAsync(src2);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _mergeRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchMerge>(), default))
            .ReturnsAsync((BatchMerge m, CancellationToken _) => m);

        var command = BuildCommand(sourceIds);
        var result = await _sut.Handle(command, default);

        result.Should().NotBeNull();
        result.NewBatchId.Should().NotBeEmpty();
        result.BatchCode.Should().StartWith("MERGED-");
    }

    [Fact]
    public async Task Handle_TwoSources_NewBatchInheritsFirstSourceOrganization()
    {
        var orgId = Guid.NewGuid();
        var src1 = BuildBatch(100m, orgId);
        var src2 = BuildBatch(80m);

        _readMock.Setup(s => s.GetByIdAsync(src1.Id, default)).ReturnsAsync(src1);
        _readMock.Setup(s => s.GetByIdAsync(src2.Id, default)).ReturnsAsync(src2);

        Batch? createdBatch = null;
        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .Callback<Batch, CancellationToken>((b, _) => createdBatch = b)
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _mergeRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchMerge>(), default))
            .ReturnsAsync((BatchMerge m, CancellationToken _) => m);

        await _sut.Handle(BuildCommand(new List<Guid> { src1.Id, src2.Id }), default);

        createdBatch!.CurrentOrganizationId.Should().Be(orgId);
    }

    [Fact]
    public async Task Handle_Valid_PersistsMergeRecord()
    {
        var orgId = Guid.NewGuid();
        var src1 = BuildBatch(orgId: orgId);
        var src2 = BuildBatch(orgId: orgId);

        _readMock.Setup(s => s.GetByIdAsync(src1.Id, default)).ReturnsAsync(src1);
        _readMock.Setup(s => s.GetByIdAsync(src2.Id, default)).ReturnsAsync(src2);

        _writeMock
            .Setup(s => s.CreateAsync(It.IsAny<Batch>(), default))
            .ReturnsAsync((Batch b, CancellationToken _) => b);

        _mergeRepoMock
            .Setup(r => r.AddAsync(It.IsAny<BatchMerge>(), default))
            .ReturnsAsync((BatchMerge m, CancellationToken _) => m);

        await _sut.Handle(BuildCommand(new List<Guid> { src1.Id, src2.Id }), default);

        _mergeRepoMock.Verify(r => r.AddAsync(It.IsAny<BatchMerge>(), default), Times.Once);
    }
}

