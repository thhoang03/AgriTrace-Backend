using AgriTrace.Application.Features.Auth.Commands;
using AgriTrace.Application.Contracts.Auth;
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

namespace AgriTrace.Tests.Application.Features.Auth;

public class LoginCommandTests
{
    private static User BuildUser(bool isActive = true)
    {
        var user = new User(null, "Test User", "test@example.com", "hash", UserRole.Staff);
        // Note: SetPassword bypasses the hash complexity since tests mock password hashing, 
        // but for verification we will mock the behavior if possible, or just use the real verify if it's simple string compare. 
        // The real VerifyPassword in User uses BCrypt, but here we can't easily mock it without a wrapper, 
        // so we'll just set a password and use the same for testing.
        user.SetPassword("CorrectPassword123!");
        if (!isActive) user.Deactivate();
        return user;
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsArgumentException()
    {
        var userMock = new Mock<IUserService>();
        userMock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);
        var tokenMock = new Mock<ITokenService>();

        var sut = new LoginCommandHandler(userMock.Object, tokenMock.Object);
        var act = () => sut.Handle(new LoginCommand("test@example.com", "pass"), default);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Email hoặc mật khẩu không đúng*");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsArgumentException()
    {
        var user = BuildUser();
        var userMock = new Mock<IUserService>();
        userMock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync(user);
        var tokenMock = new Mock<ITokenService>();

        var sut = new LoginCommandHandler(userMock.Object, tokenMock.Object);
        var act = () => sut.Handle(new LoginCommand("test@example.com", "WrongPassword"), default);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Email hoặc mật khẩu không đúng*");
    }

    [Fact]
    public async Task Handle_InactiveUser_ThrowsArgumentException()
    {
        var user = BuildUser(isActive: false);
        var userMock = new Mock<IUserService>();
        userMock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync(user);
        var tokenMock = new Mock<ITokenService>();

        var sut = new LoginCommandHandler(userMock.Object, tokenMock.Object);
        var act = () => sut.Handle(new LoginCommand("test@example.com", "CorrectPassword123!"), default);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Tài khoản đã bị vô hiệu hóa*");
    }

    [Fact]
    public async Task Handle_ValidCredentials_SetsRefreshTokenAndCallsUpdateAsync()
    {
        var user = BuildUser();
        var userMock = new Mock<IUserService>();
        userMock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync(user);
        userMock.Setup(s => s.UpdateAsync(It.IsAny<User>(), default))
            .Returns(Task.CompletedTask);

        var tokenMock = new Mock<ITokenService>();
        tokenMock.Setup(s => s.GenerateRefreshToken()).Returns("new-refresh-token");
        tokenMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");

        var sut = new LoginCommandHandler(userMock.Object, tokenMock.Object);
        var result = await sut.Handle(new LoginCommand("test@example.com", "CorrectPassword123!"), default);

        user.RefreshToken.Should().Be("new-refresh-token");
        userMock.Verify(s => s.UpdateAsync(user, default), Times.Once);
        
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
    }
}

