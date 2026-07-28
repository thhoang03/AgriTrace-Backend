using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateOrganizationValidatorTests
{
    private readonly Mock<IOrganizationTypeService> _orgTypeServiceMock = new();
    private readonly CreateOrganizationCommandValidator _validator;

    public CreateOrganizationValidatorTests()
    {
        _validator = new CreateOrganizationCommandValidator(_orgTypeServiceMock.Object);
    }
    private CreateOrganizationCommand ValidCommand()
        => new(Guid.NewGuid(), "Farm Co", "123 Address");

    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        _orgTypeServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(new OrganizationType(Guid.NewGuid(), "FARM", "Nông trại", "Mô tả", DateTime.UtcNow, null));

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
        var typeId = Guid.NewGuid();
        _orgTypeServiceMock
            .Setup(s => s.GetByIdAsync(typeId, default))
            .ReturnsAsync(new OrganizationType(typeId, "SYSTEM", "Hệ thống", "Mô tả", DateTime.UtcNow, null));

        var cmd = ValidCommand() with { OrganizationTypeId = typeId };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationTypeId);
    }

    [Fact]
    public async Task Validate_NameExceeds200Chars_HasValidationError()
    {
        var cmd = ValidCommand() with { Name = new string('A', 201) };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}