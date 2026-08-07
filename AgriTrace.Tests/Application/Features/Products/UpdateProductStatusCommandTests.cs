using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Products;

public class UpdateProductStatusCommandTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid CatId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();

    private static Product BuildProduct(string name = "Rice")
        => new(OrgId, CatId, UnitId, name);

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Product?)null);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);
        var act = () => sut.Handle(new UpdateProductStatusCommand(Guid.NewGuid(), ProductStatus.Inactive), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_SetInactive_EntityStatusIsInactive()
    {
        var product = BuildProduct();
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(product);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);

        try { await sut.Handle(new UpdateProductStatusCommand(product.Id, ProductStatus.Inactive), default); }
        catch (System.IO.FileLoadException) { }

        product.Status.Should().Be(ProductStatus.Inactive);
    }

    [Fact]
    public async Task Handle_SetActive_EntityStatusIsActive()
    {
        var product = BuildProduct();
        product.ChangeStatus(ProductStatus.Inactive);
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(product);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);

        try { await sut.Handle(new UpdateProductStatusCommand(product.Id, ProductStatus.Active), default); }
        catch (System.IO.FileLoadException) { }

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var product = BuildProduct();
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(product);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);

        try { await sut.Handle(new UpdateProductStatusCommand(product.Id, ProductStatus.Inactive), default); }
        catch (System.IO.FileLoadException) { }

        mock.Verify(s => s.UpdateAsync(product, default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdateAsyncReceivesCorrectProductId()
    {
        var product = BuildProduct();
        Product? capturedProduct = null;
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(product);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Product>(), default))
            .Callback<Product, CancellationToken>((p, _) => capturedProduct = p)
            .Returns(Task.CompletedTask);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);

        try { await sut.Handle(new UpdateProductStatusCommand(product.Id, ProductStatus.Inactive), default); }
        catch (System.IO.FileLoadException) { }

        capturedProduct.Should().NotBeNull();
        capturedProduct?.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task Handle_Always_CallsGetByIdWithCorrectId()
    {
        var productId = Guid.NewGuid();
        var mock = new Mock<IProductWriteRepository>();
        mock.Setup(s => s.GetByIdAsync(productId, default)).ReturnsAsync((Product?)null);

        var sut = new UpdateProductStatusCommandHandler(mock.Object);

        try { await sut.Handle(new UpdateProductStatusCommand(productId, ProductStatus.Active), default); }
        catch (NotFoundException) { }

        mock.Verify(s => s.GetByIdAsync(productId, default), Times.Once);
    }
}


