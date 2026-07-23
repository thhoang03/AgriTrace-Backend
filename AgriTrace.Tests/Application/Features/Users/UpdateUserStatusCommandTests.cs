using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Users.Commands;
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

namespace AgriTrace.Tests.Application.Features.Users;

/// <summary>
/// Tests for UpdateUserStatusCommandHandler.
/// </summary>
public class UpdateUserStatusCommandTests
{
    private static User BuildUser(bool isActive = true)
    {
        var user = new User(null, "Test User", "test@example.com", "hash", UserRole.Staff);
        if (!isActive) user.Deactivate();
        return user;
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((User?)null);

        var sut = new UpdateUserStatusCommandHandler(mock.Object);
        var act = () => sut.Handle(new UpdateUserStatusCommand(Guid.NewGuid(), false), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_SetInactive_UserIsDeactivated()
    {
        var user = BuildUser(isActive: true);
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByIdAsync(user.Id, default)).ReturnsAsync(user);
        mock.Setup(s => s.UpdateAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateUserStatusCommandHandler(mock.Object);
        await sut.Handle(new UpdateUserStatusCommand(user.Id, false), default);

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_SetActive_UserIsActivated()
    {
        var user = BuildUser(isActive: false);
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByIdAsync(user.Id, default)).ReturnsAsync(user);
        mock.Setup(s => s.UpdateAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateUserStatusCommandHandler(mock.Object);
        await sut.Handle(new UpdateUserStatusCommand(user.Id, true), default);

        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var user = BuildUser();
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByIdAsync(user.Id, default)).ReturnsAsync(user);
        mock.Setup(s => s.UpdateAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);

        var sut = new UpdateUserStatusCommandHandler(mock.Object);
        await sut.Handle(new UpdateUserStatusCommand(user.Id, false), default);

        mock.Verify(s => s.UpdateAsync(user, default), Times.Once);
    }
}


