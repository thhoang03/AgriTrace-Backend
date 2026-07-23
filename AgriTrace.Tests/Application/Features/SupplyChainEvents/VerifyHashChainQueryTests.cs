using AgriTrace.Application.Features.SupplyChainEvents.Queries;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.SupplyChainEvents;

public class VerifyHashChainQueryTests
{
    private readonly Mock<IEventService> _serviceMock = new();
    private readonly VerifyHashChainQueryHandler _sut;

    public VerifyHashChainQueryTests()
    {
        _sut = new VerifyHashChainQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToEventService_ReturnsTrue()
    {
        var batchId = Guid.NewGuid();
        _serviceMock
            .Setup(s => s.VerifyHashChainAsync(batchId, default))
            .ReturnsAsync(true);

        var result = await _sut.Handle(new VerifyHashChainQuery(batchId), default);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.VerifyHashChainAsync(batchId, default), Times.Once);
    }

    [Fact]
    public async Task Handle_DelegatesToEventService_ReturnsFalse()
    {
        var batchId = Guid.NewGuid();
        _serviceMock
            .Setup(s => s.VerifyHashChainAsync(batchId, default))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new VerifyHashChainQuery(batchId), default);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_PassesCorrectBatchId()
    {
        var batchId = Guid.NewGuid();
        _serviceMock
            .Setup(s => s.VerifyHashChainAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(true);

        await _sut.Handle(new VerifyHashChainQuery(batchId), default);

        _serviceMock.Verify(s => s.VerifyHashChainAsync(batchId, default), Times.Once);
    }
}
