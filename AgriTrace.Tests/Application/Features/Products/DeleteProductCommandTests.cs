using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Products;

/// <summary>
/// Tests for DeleteProductCommandHandler.
/// Handler simply calls _productWriteService.DeleteAsync(command.Id, ct).
/// </summary>
public class DeleteProductCommandTests
{
    // ── Handler delegates to service ─────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidCommand_CallsDeleteAsync()
    {
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), default)).Returns(Task.CompletedTask);

        var sut = new DeleteProductCommandHandler(mock.Object);
        await sut.Handle(new DeleteProductCommand(Guid.NewGuid()), default);

        mock.Verify(s => s.DeleteAsync(It.IsAny<Guid>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectIdToDeleteAsync()
    {
        var productId = Guid.NewGuid();
        Guid? capturedId = null;
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), default))
            .Callback<Guid, CancellationToken>((id, _) => capturedId = id)
            .Returns(Task.CompletedTask);

        var sut = new DeleteProductCommandHandler(mock.Object);
        await sut.Handle(new DeleteProductCommand(productId), default);

        capturedId.Should().Be(productId);
    }
}
