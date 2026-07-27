using AgriTrace.Application.Features.Organizations.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateOrganizationValidatorTests
{
    private readonly CreateOrganizationCommandValidator _validator = new();

    private CreateOrganizationCommand ValidCommand()
        => new(Guid.NewGuid(), "Farm Co", "123 Address");

    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyOrganizationTypeId_HasValidationError()
    {
        var cmd = ValidCommand() with { OrganizationTypeId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationTypeId);
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