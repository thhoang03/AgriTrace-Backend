using AgriTrace.Application.Features.Products.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateProductValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    private static readonly Guid ValidOrgId = Guid.NewGuid();
    private static readonly Guid ValidCategoryId = Guid.NewGuid();
    private static readonly Guid ValidUnitId = Guid.NewGuid();

    private CreateProductCommand ValidCommand()
        => new(ValidOrgId, ValidCategoryId, ValidUnitId, "Organic Rice");

    // ── Valid command ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NullCategoryId_NoErrors()
    {
        var cmd = ValidCommand() with { CategoryId = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public async Task Validate_NullUnitId_NoErrors()
    {
        var cmd = ValidCommand() with { UnitId = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.UnitId);
    }

    // ── OrganizationId ───────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyOrganizationId_HasValidationError()
    {
        var cmd = ValidCommand() with { OrganizationId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationId);
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
        var cmd = ValidCommand() with { Name = new string('A', 201) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_NameExactly200Chars_NoNameError()
    {
        var cmd = ValidCommand() with { Name = new string('A', 200) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ── CategoryId when provided ─────────────────────────────────────────────
    [Fact]
    public async Task Validate_CategoryIdIsEmptyGuid_HasValidationError()
    {
        var cmd = ValidCommand() with { CategoryId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    // ── UnitId when provided ─────────────────────────────────────────────────
    [Fact]
    public async Task Validate_UnitIdIsEmptyGuid_HasValidationError()
    {
        var cmd = ValidCommand() with { UnitId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UnitId);
    }
}
