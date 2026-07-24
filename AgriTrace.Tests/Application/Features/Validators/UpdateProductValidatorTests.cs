using AgriTrace.Application.Features.Products.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class UpdateProductValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    private static readonly Guid ValidProductId = Guid.NewGuid();

    private UpdateProductCommand ValidCommand()
        => new(ValidProductId, Guid.NewGuid(), Guid.NewGuid(), "Organic Rice");

    // ── Valid command ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── Id ───────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyId_HasValidationError()
    {
        var cmd = ValidCommand() with { Id = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    // ── Name ─────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyName_HasValidationError()
    {
        var cmd = ValidCommand() with { Name = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhitespaceName_HasValidationError()
    {
        var cmd = ValidCommand() with { Name = "   " };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_NameExceeds200Chars_HasValidationError()
    {
        var cmd = ValidCommand() with { Name = new string('B', 201) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_NameExactly200Chars_NoNameError()
    {
        var cmd = ValidCommand() with { Name = new string('B', 200) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
