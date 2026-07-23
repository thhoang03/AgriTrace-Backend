using AgriTrace.Application.Features.Users.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    private CreateUserCommand ValidCommand()
        => new(null, "Nguyen Van A", "user@example.com", "secret123", "Staff");

    // ── Valid command ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── FullName ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyFullName_HasValidationError()
    {
        var cmd = ValidCommand() with { FullName = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public async Task Validate_WhitespaceFullName_HasValidationError()
    {
        var cmd = ValidCommand() with { FullName = "   " };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public async Task Validate_FullNameExceeds200Chars_HasValidationError()
    {
        var cmd = ValidCommand() with { FullName = new string('A', 201) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    // ── Email ─────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyEmail_HasValidationError()
    {
        var cmd = ValidCommand() with { Email = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_HasValidationError()
    {
        var cmd = ValidCommand() with { Email = "not-an-email" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_ValidEmail_NoEmailError()
    {
        var cmd = ValidCommand() with { Email = "valid@domain.com" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    // ── Password ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyPassword_HasValidationError()
    {
        var cmd = ValidCommand() with { Password = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Validate_PasswordTooShort_HasValidationError()
    {
        var cmd = ValidCommand() with { Password = "abc" }; // < 6 chars
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Validate_PasswordExactlyMinLength_NoPasswordError()
    {
        var cmd = ValidCommand() with { Password = "abcdef" }; // exactly 6 chars
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    // ── Role ─────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyRole_HasValidationError()
    {
        var cmd = ValidCommand() with { Role = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}
