using AgriTrace.Application.Features.Products.Commands;
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
using NewProduct = AgriTrace.Domain.Entities.Products.NewProduct;

namespace AgriTrace.Tests.Application.Features.Products;

/// <summary>
/// Tests for CreateProductCommandHandler.
/// NOTE: Handler calls _productWriteService.CreateAsync(command.ToNewProduct(), ct)
/// which returns a Product. No Mapster involved here, so we can verify the result directly.
/// </summary>
public class CreateProductCommandTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();

    private static Product BuildProduct(string name = "Rice")
        => new(OrgId, CategoryId, UnitId, name);

    // ── Handler delegates to service ─────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.CreateAsync(It.IsAny<NewProduct>(), default))
            .ReturnsAsync(BuildProduct());

        var sut = new CreateProductCommandHandler(mock.Object);
        await sut.Handle(new CreateProductCommand(OrgId, CategoryId, UnitId, "Rice"), default);

        mock.Verify(s => s.CreateAsync(It.IsAny<NewProduct>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProduct()
    {
        var expected = BuildProduct("Rice");
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.CreateAsync(It.IsAny<NewProduct>(), default))
            .ReturnsAsync(expected);

        var sut = new CreateProductCommandHandler(mock.Object);
        var result = await sut.Handle(new CreateProductCommand(OrgId, CategoryId, UnitId, "Rice"), default);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectOrganizationId()
    {
        NewProduct? captured = null;
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.CreateAsync(It.IsAny<NewProduct>(), default))
            .Callback<NewProduct, CancellationToken>((np, _) => captured = np)
            .ReturnsAsync(BuildProduct());

        var sut = new CreateProductCommandHandler(mock.Object);
        await sut.Handle(new CreateProductCommand(OrgId, CategoryId, UnitId, "Rice"), default);

        captured.Should().NotBeNull();
        captured!.OrganizationId.Should().Be(OrgId);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectName()
    {
        NewProduct? captured = null;
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.CreateAsync(It.IsAny<NewProduct>(), default))
            .Callback<NewProduct, CancellationToken>((np, _) => captured = np)
            .ReturnsAsync(BuildProduct());

        var sut = new CreateProductCommandHandler(mock.Object);
        await sut.Handle(new CreateProductCommand(OrgId, null, null, "Organic Corn"), default);

        captured!.Name.Should().Be("Organic Corn");
    }
}

