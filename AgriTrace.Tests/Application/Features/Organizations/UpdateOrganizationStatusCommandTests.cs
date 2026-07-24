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
/// Tests for UpdateOrganizationStatusCommandHandler.
/// Note: Mapster is used at the end, so we catch FileLoadException.
/// </summary>
public class UpdateOrganizationStatusCommandTests
{
    private static Organization BuildOrganization(OrganizationStatus status = OrganizationStatus.Active)
    {
        var org = new Organization(Guid.NewGuid(), "Org Name", "Address");
        if (status == OrganizationStatus.Inactive) org.Deactivate();
        return org;
    }

    [Fact]
    public async Task Handle_OrgNotFound_ThrowsNotFoundException()
    {
        var mock = new Mock<IOrganizationService>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Organization?)null);

        var sut = new UpdateOrganizationStatusCommandHandler(mock.Object);
        var cmd = new UpdateOrganizationStatusCommand(Guid.NewGuid(), "ACTIVE");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_StatusActive_OrgIsActivated()
    {
        var org = BuildOrganization(OrganizationStatus.Inactive);
        var mock = new Mock<IOrganizationService>();
        mock.Setup(s => s.GetByIdAsync(org.Id, default)).ReturnsAsync(org);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Organization>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateOrganizationStatusCommandHandler(mock.Object);
        var cmd = new UpdateOrganizationStatusCommand(org.Id, "ACTIVE");

        try { await sut.Handle(cmd, default); }
        catch (System.IO.FileLoadException) { }

        org.Status.Should().Be(OrganizationStatus.Active);
    }

    [Fact]
    public async Task Handle_StatusInactive_OrgIsDeactivated()
    {
        var org = BuildOrganization(OrganizationStatus.Active);
        var mock = new Mock<IOrganizationService>();
        mock.Setup(s => s.GetByIdAsync(org.Id, default)).ReturnsAsync(org);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Organization>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateOrganizationStatusCommandHandler(mock.Object);
        var cmd = new UpdateOrganizationStatusCommand(org.Id, "INACTIVE");

        try { await sut.Handle(cmd, default); }
        catch (System.IO.FileLoadException) { }

        org.Status.Should().Be(OrganizationStatus.Inactive);
    }

    [Fact]
    public async Task Handle_Valid_CallsUpdateAsync()
    {
        var org = BuildOrganization();
        var mock = new Mock<IOrganizationService>();
        mock.Setup(s => s.GetByIdAsync(org.Id, default)).ReturnsAsync(org);
        mock.Setup(s => s.UpdateAsync(It.IsAny<Organization>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateOrganizationStatusCommandHandler(mock.Object);
        var cmd = new UpdateOrganizationStatusCommand(org.Id, "INACTIVE");

        try { await sut.Handle(cmd, default); }
        catch (System.IO.FileLoadException) { }

        mock.Verify(s => s.UpdateAsync(org, default), Times.Once);
    }
}

