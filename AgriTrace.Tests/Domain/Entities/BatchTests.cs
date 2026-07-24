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

public class BatchTests
{
    // ── Helpers ─────────────────────────────────────────────────────────────
    private static readonly Guid ValidProductId = Guid.NewGuid();
    private static readonly Guid ValidUnitId = Guid.NewGuid();
    private const string ValidCode = "BATCH001";
    private static readonly DateTime ValidProduction = new(2025, 1, 1);
    private static readonly DateTime ValidExpiry = new(2025, 12, 31);

    private static Batch CreateValid(
        decimal quantity = 100m,
        DateTime? expiryDate = null)
        => new(ValidProductId, ValidCode, quantity, ValidUnitId, ValidProduction, expiryDate ?? ValidExpiry);

    // ── Constructor — happy path ─────────────────────────────────────────────
    [Fact]
    public void Constructor_ValidParams_SetsAllProperties()
    {
        var batch = CreateValid();

        batch.ProductId.Should().Be(ValidProductId);
        batch.BatchCode.Should().Be(ValidCode);
        batch.Quantity.Should().Be(100m);
        batch.RemainingQuantity.Should().Be(100m);
        batch.SourceQuantity.Should().Be(100m);
        batch.UnitId.Should().Be(ValidUnitId);
        batch.Status.Should().Be(BatchStatus.Created);
    }

    [Fact]
    public void Constructor_NewBatch_RootBatchIdEqualsId()
    {
        var batch = CreateValid();
        batch.RootBatchId.Should().Be(batch.Id);
    }

    [Fact]
    public void Constructor_BatchCode_IsTrimmed()
    {
        var batch = new Batch(ValidProductId, "  ABC  ", 10m, ValidUnitId, ValidProduction, null);
        batch.BatchCode.Should().Be("ABC");
    }

    // ── Constructor — guard clauses ──────────────────────────────────────────
    [Fact]
    public void Constructor_EmptyProductId_Throws()
    {
        var act = () => new Batch(Guid.Empty, ValidCode, 100m, ValidUnitId, ValidProduction, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyUnitId_Throws()
    {
        var act = () => new Batch(ValidProductId, ValidCode, 100m, Guid.Empty, ValidProduction, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyBatchCode_Throws()
    {
        var act = () => new Batch(ValidProductId, "   ", 100m, ValidUnitId, ValidProduction, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ZeroQuantity_Throws()
    {
        var act = () => new Batch(ValidProductId, ValidCode, 0m, ValidUnitId, ValidProduction, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_NegativeQuantity_Throws()
    {
        var act = () => new Batch(ValidProductId, ValidCode, -1m, ValidUnitId, ValidProduction, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ExpiryBeforeProduction_Throws()
    {
        var production = new DateTime(2025, 6, 1);
        var expiry = new DateTime(2025, 1, 1);
        var act = () => new Batch(ValidProductId, ValidCode, 100m, ValidUnitId, production, expiry);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_NullExpiryDate_DoesNotThrow()
    {
        var act = () => new Batch(ValidProductId, ValidCode, 100m, ValidUnitId, ValidProduction, null);
        act.Should().NotThrow();
    }

    // ── ReduceRemainingQuantity ──────────────────────────────────────────────
    [Fact]
    public void ReduceRemainingQuantity_Valid_DecreasesCorrectly()
    {
        var batch = CreateValid(100m);
        batch.ReduceRemainingQuantity(30m);
        batch.RemainingQuantity.Should().Be(70m);
    }

    [Fact]
    public void ReduceRemainingQuantity_ZeroQty_Throws()
    {
        var batch = CreateValid(100m);
        var act = () => batch.ReduceRemainingQuantity(0m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReduceRemainingQuantity_NegativeQty_Throws()
    {
        var batch = CreateValid(100m);
        var act = () => batch.ReduceRemainingQuantity(-5m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReduceRemainingQuantity_ExceedsRemaining_ThrowsInvalidOperation()
    {
        var batch = CreateValid(100m);
        var act = () => batch.ReduceRemainingQuantity(101m);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not enough*");
    }

    [Fact]
    public void ReduceRemainingQuantity_ExactlyRemaining_ReducesToZero()
    {
        var batch = CreateValid(100m);
        batch.ReduceRemainingQuantity(100m);
        batch.RemainingQuantity.Should().Be(0m);
    }

    // ── CreateChildBatch ─────────────────────────────────────────────────────
    [Fact]
    public void CreateChildBatch_Valid_ReturnsChildWithCorrectLinks()
    {
        var parent = CreateValid(100m);
        var splitId = Guid.NewGuid();

        var child = parent.CreateChildBatch("CHILD01", 40m, splitId);

        child.ParentBatchId.Should().Be(parent.Id);
        child.RootBatchId.Should().Be(parent.RootBatchId);
        child.SplitId.Should().Be(splitId);
        child.Quantity.Should().Be(40m);
        child.CurrentOrganizationId.Should().Be(parent.CurrentOrganizationId);
    }

    [Fact]
    public void CreateChildBatch_Valid_ReducesParentRemaining()
    {
        var parent = CreateValid(100m);
        parent.CreateChildBatch("CHILD01", 40m, Guid.NewGuid());
        parent.RemainingQuantity.Should().Be(60m);
    }

    [Fact]
    public void CreateChildBatch_ZeroQty_Throws()
    {
        var parent = CreateValid(100m);
        var act = () => parent.CreateChildBatch("CHILD01", 0m, Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateChildBatch_ExceedsRemaining_ThrowsInvalidOperation()
    {
        var parent = CreateValid(100m);
        var act = () => parent.CreateChildBatch("CHILD01", 200m, Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>();
    }

    // ── UpdateInformation ────────────────────────────────────────────────────
    [Fact]
    public void UpdateInformation_QuantityEqualsRemaining_UpdatesBoth()
    {
        var batch = CreateValid(100m);
        // Initially Quantity == RemainingQuantity == 100
        batch.UpdateInformation(ValidCode, 150m, ValidProduction, ValidExpiry);

        batch.Quantity.Should().Be(150m);
        batch.RemainingQuantity.Should().Be(150m);
    }

    [Fact]
    public void UpdateInformation_QuantityNotEqualRemaining_OnlyUpdatesQuantity()
    {
        var batch = CreateValid(100m);
        batch.ReduceRemainingQuantity(30m); // remaining = 70
        batch.UpdateInformation(ValidCode, 150m, ValidProduction, ValidExpiry);

        batch.Quantity.Should().Be(150m);
        batch.RemainingQuantity.Should().Be(70m); // unchanged
    }

    // ── ChangeStatus / Recall ─────────────────────────────────────────────────
    [Fact]
    public void ChangeStatus_SetsStatusCorrectly()
    {
        var batch = CreateValid();
        batch.ChangeStatus(BatchStatus.Transporting);
        batch.Status.Should().Be(BatchStatus.Transporting);
    }

    [Fact]
    public void Recall_SetsStatusToRecalled()
    {
        var batch = CreateValid();
        batch.Recall();
        batch.Status.Should().Be(BatchStatus.Recalled);
    }

    [Fact]
    public void CompleteProduction_SetsStatusToTransporting()
    {
        var batch = CreateValid();
        batch.CompleteProduction();
        batch.Status.Should().Be(BatchStatus.Transporting);
    }

    // ── ChangeOrganization ──────────────────────────────────────────────────
    [Fact]
    public void ChangeOrganization_Valid_SetsOrganization()
    {
        var batch = CreateValid();
        var orgId = Guid.NewGuid();
        batch.ChangeOrganization(orgId);
        batch.CurrentOrganizationId.Should().Be(orgId);
    }

    [Fact]
    public void ChangeOrganization_EmptyGuid_Throws()
    {
        var batch = CreateValid();
        var act = () => batch.ChangeOrganization(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    // ── SetQRCode ────────────────────────────────────────────────────────────
    [Fact]
    public void SetQRCode_SetsValue()
    {
        var batch = CreateValid();
        batch.SetQRCode("https://agritrace.io/qr/abc");
        batch.QRCode.Should().Be("https://agritrace.io/qr/abc");
    }

    // ── RestoreRemainingQuantity ─────────────────────────────────────────────
    [Fact]
    public void RestoreRemainingQuantity_Valid_SetsQuantity()
    {
        var batch = CreateValid(100m);
        batch.ReduceRemainingQuantity(50m);
        batch.RestoreRemainingQuantity(80m);
        batch.RemainingQuantity.Should().Be(80m);
    }

    [Fact]
    public void RestoreRemainingQuantity_Negative_Throws()
    {
        var batch = CreateValid(100m);
        var act = () => batch.RestoreRemainingQuantity(-1m);
        act.Should().Throw<ArgumentException>();
    }

    // ── UpdatedAt ────────────────────────────────────────────────────────────
    [Fact]
    public void ChangeStatus_SetsUpdatedAt()
    {
        var batch = CreateValid();
        var before = DateTime.UtcNow;
        batch.ChangeStatus(BatchStatus.Harvested);
        batch.UpdatedAt.Should().NotBeNull();
        batch.UpdatedAt.Should().BeOnOrAfter(before);
    }
}

