using AgriTrace.Application.Features.Batches.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class UpdateBatchValidatorTests
{
    private readonly UpdateBatchCommandValidator _validator = new();

    private static readonly Guid ValidId = Guid.NewGuid();

    private UpdateBatchCommand ValidCommand()
        => new(ValidId, 100m, null);

    // ── Valid command ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NullQuantity_NoQuantityError()
    {
        var cmd = ValidCommand() with { Quantity = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    // ── Id ───────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyId_HasValidationError()
    {
        var cmd = ValidCommand() with { Id = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    // ── Quantity ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ZeroQuantity_HasValidationError()
    {
        var cmd = ValidCommand() with { Quantity = 0m };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Validate_NegativeQuantity_HasValidationError()
    {
        var cmd = ValidCommand() with { Quantity = -1m };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Validate_PositiveQuantity_NoQuantityError()
    {
        var cmd = ValidCommand() with { Quantity = 0.001m };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }
}
