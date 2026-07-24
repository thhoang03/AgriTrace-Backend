using AgriTrace.Application.Features.SupplyChainEvents.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateSupplyChainEventValidatorTests
{
    private readonly CreateSupplyChainEventCommandValidator _validator = new();

    private static readonly Guid ValidBatchId = Guid.NewGuid();
    private static readonly Guid ValidEventTypeId = Guid.NewGuid();
    private static readonly Guid ValidOrgId = Guid.NewGuid();
    private static readonly Guid ValidUserId = Guid.NewGuid();

    private CreateSupplyChainEventCommand ValidCommand()
        => new(ValidBatchId, ValidEventTypeId, ValidOrgId, ValidUserId, "data", "location");

    // ── Valid ─────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── BatchId ───────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyBatchId_HasValidationError()
    {
        var cmd = ValidCommand() with { BatchId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.BatchId);
    }

    // ── EventTypeId ───────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyEventTypeId_HasValidationError()
    {
        var cmd = ValidCommand() with { EventTypeId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.EventTypeId);
    }

    // ── OrganizationId ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyOrganizationId_HasValidationError()
    {
        var cmd = ValidCommand() with { OrganizationId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationId);
    }

    // ── PerformedByUserId ────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyPerformedByUserId_HasValidationError()
    {
        var cmd = ValidCommand() with { PerformedByUserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PerformedByUserId);
    }

    // ── EventData ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EventDataExceedsMaxLength_HasValidationError()
    {
        var longData = new string('x', 4001);
        var cmd = ValidCommand() with { EventData = longData };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.EventData);
    }

    [Fact]
    public async Task Validate_EventDataExactlyMaxLength_NoError()
    {
        var maxData = new string('x', 4000);
        var cmd = ValidCommand() with { EventData = maxData };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.EventData);
    }

    [Fact]
    public async Task Validate_NullEventData_NoError()
    {
        var cmd = ValidCommand() with { EventData = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.EventData);
    }

    // ── Location ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_LocationExceedsMaxLength_HasValidationError()
    {
        var longLoc = new string('a', 501);
        var cmd = ValidCommand() with { Location = longLoc };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public async Task Validate_LocationExactlyMaxLength_NoError()
    {
        var maxLoc = new string('a', 500);
        var cmd = ValidCommand() with { Location = maxLoc };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public async Task Validate_NullLocation_NoError()
    {
        var cmd = ValidCommand() with { Location = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Location);
    }
}
