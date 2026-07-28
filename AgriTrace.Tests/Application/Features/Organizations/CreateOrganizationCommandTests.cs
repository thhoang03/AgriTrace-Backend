using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Organizations;

/// <summary>
/// Tests for CreateOrganizationCommandHandler.
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
        var orgTypeId = Guid.NewGuid();
        var dummyOrgType = new OrganizationType(orgTypeId, "FARM", "Nông trại", "Mô tả", DateTime.UtcNow, null);
        var existing = BuildOrganization("Dup Name");
        var mockOrgService = new Mock<IOrganizationService>();
        var mockOrgTypeService = new Mock<IOrganizationTypeService>();
        var mockOrgCurrentUser = new Mock<ICurrentUserService>();

        mockOrgService.Setup(s => s.GetByNameAsync("Dup Name", default))
          .ReturnsAsync(existing);
        mockOrgTypeService.Setup(s => s.GetByIdAsync(orgTypeId, default))
        .ReturnsAsync(dummyOrgType);

        var sut = new CreateOrganizationCommandHandler(mockOrgService.Object, BuildCurrentUser().Object, mockOrgTypeService.Object);
        //var sut = new CreateOrganizationCommandHandler(orgMock.Object, BuildCurrentUser().Object);
        var cmd = new CreateOrganizationCommand(orgTypeId, "Dup Name", "Address");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var orgTypeId = Guid.NewGuid();
        var mockOrgService = new Mock<IOrganizationService>();
        var mockOrgTypeService = new Mock<IOrganizationTypeService>();

        var dummyOrgType = new OrganizationType(orgTypeId, "FARM", "Nông trại", "Mô tả", DateTime.UtcNow, null);

        mockOrgService.Setup(s => s.GetByNameAsync(It.IsAny<string>(), default))
            .ReturnsAsync((Organization?)null);

        mockOrgTypeService.Setup(s => s.GetByIdAsync(orgTypeId, default))
            .ReturnsAsync(dummyOrgType);

        mockOrgService.Setup(s => s.CreateAsync(It.IsAny<Organization>(), default))
            .ReturnsAsync((Organization o, CancellationToken _) => o);

        var sut = new CreateOrganizationCommandHandler(mockOrgService.Object, BuildCurrentUser().Object,mockOrgTypeService.Object);
        var cmd = new CreateOrganizationCommand(orgTypeId, "New Org", "Addr");

        var result = await sut.Handle(cmd, default);

        mockOrgService.Verify(s => s.CreateAsync(It.IsAny<Organization>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_SystemType_ThrowsRbacForbiddenException()
    {
        var orgTypeId = Guid.NewGuid();
        var mockOrgService = new Mock<IOrganizationService>();
        var mockOrgTypeService=new Mock<IOrganizationTypeService>();

        var dummyOrgType = new OrganizationType(orgTypeId, "SYSTEM", "Hệ thống", "Mô tả", DateTime.UtcNow, null);

        mockOrgService.Setup(s => s.GetByNameAsync(It.IsAny<string>(), default))
            .ReturnsAsync((Organization?)null);

        mockOrgTypeService.Setup(s => s.GetByIdAsync(orgTypeId, default))
            .ReturnsAsync(dummyOrgType);


        var sut = new CreateOrganizationCommandHandler(mockOrgService.Object, BuildCurrentUser().Object,mockOrgTypeService.Object);
        var cmd = new CreateOrganizationCommand(orgTypeId, "System Org", "Address");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<RbacForbiddenException>()
            .WithMessage("*SYSTEM*");
    }
}
