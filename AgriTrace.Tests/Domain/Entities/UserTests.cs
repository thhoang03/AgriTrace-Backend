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
using FluentAssertions;

namespace AgriTrace.Tests.Domain.Entities;

public class UserTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────
    private static User CreateValid(
        string fullName = "Nguyen Van A",
        string email = "user@example.com",
        string passwordHash = "100000.saltB64.keyB64",
        UserRole role = UserRole.Staff)
        => new(null, fullName, email, passwordHash, role);

    // ── Constructor — happy path ─────────────────────────────────────────────
    [Fact]
    public void Constructor_ValidParams_SetsProperties()
    {
        var user = CreateValid();

        user.FullName.Should().Be("Nguyen Van A");
        user.Email.Should().Be("user@example.com");
        user.Role.Should().Be(UserRole.Staff);
        user.IsActive.Should().BeTrue();
        user.OrganizationId.Should().BeNull();
    }

    [Fact]
    public void Constructor_Email_IsLowercased()
    {
        var user = new User(null, "Test User", "USER@EXAMPLE.COM", "hash.salt.key", UserRole.Staff);
        user.Email.Should().Be("user@example.com");
    }

    [Fact]
    public void Constructor_FullName_IsTrimmed()
    {
        var user = new User(null, "  Nguyen Van A  ", "a@b.com", "hash.salt.key", UserRole.Staff);
        user.FullName.Should().Be("Nguyen Van A");
    }

    [Fact]
    public void Constructor_WithOrganizationId_SetsIt()
    {
        var orgId = Guid.NewGuid();
        var user = new User(orgId, "Name", "a@b.com", "hash.salt.key", UserRole.Manager);
        user.OrganizationId.Should().Be(orgId);
    }

    // ── Constructor — guard clauses ──────────────────────────────────────────
    [Fact]
    public void Constructor_EmptyFullName_Throws()
    {
        var act = () => new User(null, "   ", "a@b.com", "hash.salt.key", UserRole.Staff);
        act.Should().Throw<ArgumentException>().WithMessage("*Full name*");
    }

    [Fact]
    public void Constructor_NullFullName_Throws()
    {
        var act = () => new User(null, null!, "a@b.com", "hash.salt.key", UserRole.Staff);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmailWithoutAtSign_Throws()
    {
        var act = () => new User(null, "Name", "invalidemail", "hash.salt.key", UserRole.Staff);
        act.Should().Throw<ArgumentException>().WithMessage("*Email*");
    }

    [Fact]
    public void Constructor_EmptyEmail_Throws()
    {
        var act = () => new User(null, "Name", "   ", "hash.salt.key", UserRole.Staff);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyPasswordHash_Throws()
    {
        var act = () => new User(null, "Name", "a@b.com", "   ", UserRole.Staff);
        act.Should().Throw<ArgumentException>().WithMessage("*Password*");
    }

    // ── SetPassword / VerifyPassword ─────────────────────────────────────────
    [Fact]
    public void SetPassword_EmptyPassword_Throws()
    {
        var user = CreateValid();
        var act = () => user.SetPassword("   ");
        act.Should().Throw<ArgumentException>().WithMessage("*Password*");
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var user = CreateValid();
        user.SetPassword("MySecret123!");
        user.VerifyPassword("MySecret123!").Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var user = CreateValid();
        user.SetPassword("MySecret123!");
        user.VerifyPassword("WrongPassword").Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_EmptyInput_ReturnsFalse()
    {
        var user = CreateValid();
        user.SetPassword("MySecret123!");
        user.VerifyPassword("").Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_InvalidHashFormat_ReturnsFalse()
    {
        // If PasswordHash doesn't have 3 dot-separated parts, should return false safely
        var user = new User(null, "Name", "a@b.com", "not_a_valid_hash_format", UserRole.Staff);
        user.VerifyPassword("any").Should().BeFalse();
    }

    // ── UpdateProfileDetails ──────────────────────────────────────────────────
    [Fact]
    public void UpdateProfileDetails_OnlyFullName_UpdatesOnlyFullName()
    {
        var user = CreateValid();
        var originalEmail = user.Email;
        var originalRole = user.Role;

        user.UpdateProfileDetails("New Name", null, null);

        user.FullName.Should().Be("New Name");
        user.Email.Should().Be(originalEmail);
        user.Role.Should().Be(originalRole);
    }

    [Fact]
    public void UpdateProfileDetails_OnlyPhone_UpdatesPhone()
    {
        var user = CreateValid();
        user.UpdateProfileDetails(null, "0901234567", null);
        user.Phone.Should().Be("0901234567");
    }

    [Fact]
    public void UpdateProfileDetails_OnlyRole_UpdatesRole()
    {
        var user = CreateValid(role: UserRole.Staff);
        user.UpdateProfileDetails(null, null, UserRole.Staff);
        user.Role.Should().Be(UserRole.Staff);
    }

    [Fact]
    public void UpdateProfileDetails_NullAll_ChangesNothing()
    {
        var user = CreateValid();
        var name = user.FullName;
        user.UpdateProfileDetails(null, null, null);
        user.FullName.Should().Be(name);
    }

    [Fact]
    public void UpdateProfileDetails_WhitespaceName_SkipsUpdate()
    {
        var user = CreateValid();
        var originalName = user.FullName;
        user.UpdateProfileDetails("   ", null, null);
        user.FullName.Should().Be(originalName);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────
    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var user = CreateValid();
        user.Deactivate();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_AfterDeactivate_SetsIsActiveTrue()
    {
        var user = CreateValid();
        user.Deactivate();
        user.Activate();
        user.IsActive.Should().BeTrue();
    }

    // ── ChangePassword ────────────────────────────────────────────────────────
    [Fact]
    public void ChangePassword_EmptyHash_Throws()
    {
        var user = CreateValid();
        var act = () => user.ChangePassword("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ChangePassword_ValidHash_UpdatesPasswordHash()
    {
        var user = CreateValid();
        user.ChangePassword("newHash.salt.key");
        user.PasswordHash.Should().Be("newHash.salt.key");
    }

    // ── RefreshToken ──────────────────────────────────────────────────────────
    [Fact]
    public void SetRefreshToken_SetsTokenAndExpiry()
    {
        var user = CreateValid();
        var expiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken("my-refresh-token", expiry);

        user.RefreshToken.Should().Be("my-refresh-token");
        user.RefreshTokenExpiry.Should().BeCloseTo(expiry, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsRefreshTokenValid_ValidTokenNotExpired_ReturnsTrue()
    {
        var user = CreateValid();
        var expiry = DateTime.UtcNow.AddHours(1);
        user.SetRefreshToken("token123", expiry);
        user.IsRefreshTokenValid("token123").Should().BeTrue();
    }

    [Fact]
    public void IsRefreshTokenValid_WrongToken_ReturnsFalse()
    {
        var user = CreateValid();
        user.SetRefreshToken("token123", DateTime.UtcNow.AddHours(1));
        user.IsRefreshTokenValid("wrong-token").Should().BeFalse();
    }

    [Fact]
    public void IsRefreshTokenValid_ExpiredToken_ReturnsFalse()
    {
        var user = CreateValid();
        user.SetRefreshToken("token123", DateTime.UtcNow.AddSeconds(-1));
        user.IsRefreshTokenValid("token123").Should().BeFalse();
    }

    [Fact]
    public void ClearRefreshToken_SetsTokenAndExpiryToNull()
    {
        var user = CreateValid();
        user.SetRefreshToken("token", DateTime.UtcNow.AddDays(1));
        user.ClearRefreshToken();

        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiry.Should().BeNull();
    }

    // ── UpdatedAt ─────────────────────────────────────────────────────────────
    [Fact]
    public void Deactivate_SetsUpdatedAt()
    {
        var user = CreateValid();
        var before = DateTime.UtcNow;
        user.Deactivate();
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeOnOrAfter(before);
    }
}

