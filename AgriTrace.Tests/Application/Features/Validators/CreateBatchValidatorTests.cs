using AgriTrace.Application.Features.Batches.Commands;
using FluentValidation.TestHelper;

namespace AgriTrace.Tests.Application.Features.Validators;

public class CreateBatchValidatorTests
{
    private readonly CreateBatchCommandValidator _validator = new();

    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly Guid ValidUnitId = Guid.NewGuid();
    private static readonly DateTime ValidProduction = new(2025, 1, 1);
    private static readonly DateTime ValidExpiry = new(2026, 1, 1);

    private CreateBatchCommand ValidCommand()
        => new(ValidProductId, ValidUnitId, 100m, ValidProduction, ValidExpiry);

    // ── Valid command ────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ValidCommand_NoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── ProductId ────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyProductId_HasValidationError()
    {
        var cmd = ValidCommand() with { ProductId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    // ── UnitId ───────────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_EmptyUnitId_HasValidationError()
    {
        var cmd = ValidCommand() with { UnitId = Guid.Empty };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UnitId);
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

    // ── ProductionDate ───────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_DefaultProductionDate_HasValidationError()
    {
        var cmd = ValidCommand() with { ProductionDate = default };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductionDate);
    }

    // ── ExpiryDate ───────────────────────────────────────────────────────────
    [Fact]
    public async Task Validate_ExpiryBeforeProduction_HasValidationError()
    {
        var cmd = ValidCommand() with
        {
            ProductionDate = new DateTime(2025, 6, 1),
            ExpiryDate = new DateTime(2025, 1, 1)
        };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ExpiryDate);
    }

    [Fact]
    public async Task Validate_NullExpiryDate_NoExpiryError()
    {
        var cmd = ValidCommand() with { ExpiryDate = null };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryDate);
    }

    [Fact]
    public async Task Validate_ExpiryAfterProduction_NoExpiryError()
    {
        var cmd = ValidCommand() with
        {
            ProductionDate = new DateTime(2025, 1, 1),
            ExpiryDate = new DateTime(2026, 1, 1)
        };
        var result = await _validator.TestValidateAsync(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryDate);
    }
}
