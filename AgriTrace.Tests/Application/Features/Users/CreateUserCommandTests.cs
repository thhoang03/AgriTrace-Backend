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
/// Tests for CreateUserCommandHandler.
/// Handler logic: validate role enum → check duplicate email → create user → set password → CreateAsync → ToDto.
/// ToDto() does not use Mapster (manual mapping), so no FileLoadException expected.
/// </summary>
public class CreateUserCommandTests
{
    private static User BuildUser(string email = "test@example.com")
    {
        var user = new User(null, "Test User", email, "100000.salt.key", UserRole.Staff);
        return user;
    }

    // ── Invalid role ─────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_InvalidRole_ThrowsArgumentException()
    {
        var mock = new Mock<IUserService>();
        var sut = new CreateUserCommandHandler(mock.Object);
        var cmd = new CreateUserCommand(null, "Name", "a@b.com", "password123", "INVALID_ROLE");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*invalid*");
    }

    // ── Duplicate email ───────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsConflictException()
    {
        var existing = BuildUser("dup@example.com");
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByEmailAsync("dup@example.com", default))
            .ReturnsAsync(existing);

        var sut = new CreateUserCommandHandler(mock.Object);
        var cmd = new CreateUserCommand(null, "Name", "DUP@example.com", "password123", "Staff");

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<ConflictException>();
    }

    // ── Happy path ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_ValidCommand_CallsCreateAsync()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);
        mock.Setup(s => s.CreateAsync(It.IsAny<User>(), default))
            .ReturnsAsync((User u, CancellationToken _) => u);

        var sut = new CreateUserCommandHandler(mock.Object);
        await sut.Handle(new CreateUserCommand(null, "Nguyen Van A", "a@b.com", "secret123", "Staff"), default);

        mock.Verify(s => s.CreateAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_UserEmailIsLowercased()
    {
        User? captured = null;
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);
        mock.Setup(s => s.CreateAsync(It.IsAny<User>(), default))
            .Callback<User, CancellationToken>((u, _) => captured = u)
            .ReturnsAsync((User u, CancellationToken _) => u);

        var sut = new CreateUserCommandHandler(mock.Object);
        await sut.Handle(new CreateUserCommand(null, "Name", "User@Example.COM", "secret123", "Staff"), default);

        captured!.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUserDtoWithCorrectEmail()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);
        mock.Setup(s => s.CreateAsync(It.IsAny<User>(), default))
            .ReturnsAsync((User u, CancellationToken _) => u);

        var sut = new CreateUserCommandHandler(mock.Object);
        var result = await sut.Handle(
            new CreateUserCommand(null, "Nguyen Van A", "a@b.com", "secret123", "Staff"), default);

        result.Should().NotBeNull();
        result.Email.Should().Be("a@b.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_LooksUpEmailBeforeCreating()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.GetByEmailAsync("check@b.com", default))
            .ReturnsAsync((User?)null);
        mock.Setup(s => s.CreateAsync(It.IsAny<User>(), default))
            .ReturnsAsync((User u, CancellationToken _) => u);

        var sut = new CreateUserCommandHandler(mock.Object);
        await sut.Handle(new CreateUserCommand(null, "Name", "check@b.com", "pass123", "Manager"), default);

        mock.Verify(s => s.GetByEmailAsync("check@b.com", default), Times.Once);
    }
}


