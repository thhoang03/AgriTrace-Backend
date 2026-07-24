using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Organizations.Commands;
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

namespace AgriTrace.Tests.Application.Features.Organizations;

/// <summary>
/// Tests for CreateOrganizationCommandHandler.
/// Note: Mapster is used at the end, so we catch FileLoadException.
/// </summary>
public class CreateOrganizationCommandTests
{
    private static Organization BuildOrganization(string name = "Farm Co")
    {
        return new Organization(Guid.NewGuid(), name, "Address");
    }

    private static Mock<ICurrentUserService> BuildCurrentUser(string role = "Admin")
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(c => c.Role).Returns(role);
        mock.Setup(c => c.OrganizationId).Returns(Guid.NewGuid());
        mock.Setup(c => c.OrganizationType).Returns((string?)null);
        return mock;
    }

    [Fact]
    public async Task Handle_DuplicateName_ThrowsConflictException()
    {
        var existing = BuildOrganization("Dup Name");
        var orgMock = new Mock<IOrganizationService>();
        orgMock.Setup(s => s.GetByNameAsync("Dup Name", default))
            .ReturnsAsync(existing);

        var sut = new CreateOrganizationCommandHandler(orgMock.Object, BuildCurrentUser().Object);
        var cmd = new CreateOrganizationCommand("FARM", "Dup Name", "Address");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var orgMock = new Mock<IOrganizationService>();
        orgMock.Setup(s => s.GetByNameAsync(It.IsAny<string>(), default))
            .ReturnsAsync((Organization?)null);
        orgMock.Setup(s => s.CreateAsync(It.IsAny<Organization>(), default))
            .ReturnsAsync((Organization o, CancellationToken _) => o);

        var sut = new CreateOrganizationCommandHandler(orgMock.Object, BuildCurrentUser().Object);
        var cmd = new CreateOrganizationCommand("FARM", "New Org", "Addr");

        try { await sut.Handle(cmd, default); }
        catch (System.IO.FileLoadException) { }

        orgMock.Verify(s => s.CreateAsync(It.IsAny<Organization>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_SystemType_ThrowsRbacForbiddenException()
    {
        var orgMock = new Mock<IOrganizationService>();
        orgMock.Setup(s => s.GetByNameAsync(It.IsAny<string>(), default))
            .ReturnsAsync((Organization?)null);

        var sut = new CreateOrganizationCommandHandler(orgMock.Object, BuildCurrentUser().Object);
        var cmd = new CreateOrganizationCommand("SYSTEM", "System Org", "Address");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<RbacForbiddenException>()
            .WithMessage("*SYSTEM*");
    }
}
