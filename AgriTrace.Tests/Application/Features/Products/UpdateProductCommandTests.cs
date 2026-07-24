using AgriTrace.Application.Common.Exceptions;
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
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Products;

/// <summary>
/// Tests for UpdateProductCommandHandler.
/// Handler: fetches product by id (via write service), calls product.UpdateInformation,
/// then calls UpdateAsync. No Mapster involved.
/// </summary>
public class UpdateProductCommandTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid CatId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();

    private static Product BuildProduct(string name = "Rice")
        => new(OrgId, CatId, UnitId, name);

    private static UpdateProductCommandHandler Build(
        Mock<IProductWriteService> mock)
        => new(mock.Object);

    // ── Handler calls UpdateAsync ─────────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProduct>(), default))
            .Returns(Task.CompletedTask);

        var sut = Build(mock);
        await sut.Handle(new UpdateProductCommand(Guid.NewGuid(), CatId, UnitId, "New Name"), default);

        mock.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProduct>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectIdToUpdateAsync()
    {
        var productId = Guid.NewGuid();
        Guid? capturedId = null;
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProduct>(), default))
            .Callback<Guid, UpdateProduct, CancellationToken>((id, _, _) => capturedId = id)
            .Returns(Task.CompletedTask);

        var sut = Build(mock);
        await sut.Handle(new UpdateProductCommand(productId, null, null, "Name"), default);

        capturedId.Should().Be(productId);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectNameToUpdateAsync()
    {
        UpdateProduct? captured = null;
        var mock = new Mock<IProductWriteService>();
        mock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProduct>(), default))
            .Callback<Guid, UpdateProduct, CancellationToken>((_, up, _) => captured = up)
            .Returns(Task.CompletedTask);

        var sut = Build(mock);
        await sut.Handle(new UpdateProductCommand(Guid.NewGuid(), null, null, "Brown Rice"), default);

        captured!.Name.Should().Be("Brown Rice");
    }
}

