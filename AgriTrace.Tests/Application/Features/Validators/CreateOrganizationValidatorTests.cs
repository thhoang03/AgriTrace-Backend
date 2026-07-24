using AgriTrace.Application.Features.Organizations.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateOrganizationValidatorTests
{
    private readonly CreateOrganizationCommandValidator _validator = new();

    private CreateOrganizationCommand ValidCommand()
        => new("FARM", "Farm Co", "123 Address");

    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyType_HasValidationError()
    {
        var cmd = ValidCommand() with { Type = "" };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Theory]
    [InlineData("INVALID_TYPE")]
    [InlineData("farm")] // case-sensitive in the current implementation
    public async Task Validate_InvalidType_HasValidationError(string type)
    {
        var cmd = ValidCommand() with { Type = type };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Theory]
    [InlineData("FARM")]
    [InlineData("PROCESSOR")]
    [InlineData("DISTRIBUTOR")]
    [InlineData("RETAILER")]
    [InlineData("INSPECTOR_ORG")]
    public async Task Validate_ValidTypes_NoErrors(string type)
    {
        var cmd = ValidCommand() with { Type = type };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public async Task Validate_EmptyName_HasValidationError()
    {
        var cmd = ValidCommand() with { Name = "" };
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
}
