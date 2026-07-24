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

public class ProductTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────
    private static readonly Guid ValidOrgId = Guid.NewGuid();
    private static readonly Guid ValidCategoryId = Guid.NewGuid();
    private static readonly Guid ValidUnitId = Guid.NewGuid();
    private const string ValidName = "Organic Rice";

    private static Product CreateValid(
        string name = ValidName,
        Guid? categoryId = null,
        Guid? unitId = null)
        => new(ValidOrgId, categoryId ?? ValidCategoryId, unitId ?? ValidUnitId, name);

    // ── Constructor — happy path ─────────────────────────────────────────────
    [Fact]
    public void Constructor_ValidParams_SetsAllProperties()
    {
        var product = CreateValid();

        product.OrganizationId.Should().Be(ValidOrgId);
        product.CategoryId.Should().Be(ValidCategoryId);
        product.UnitId.Should().Be(ValidUnitId);
        product.Name.Should().Be(ValidName);
    }

    [Fact]
    public void Constructor_ValidParams_StatusDefaultsToCreated()
    {
        var product = CreateValid();
        product.Status.Should().Be(ProductStatus.Created);
    }

    [Fact]
    public void Constructor_NameWithWhitespace_IsTrimmed()
    {
        var product = new Product(ValidOrgId, null, null, "  Corn  ");
        product.Name.Should().Be("Corn");
    }

    [Fact]
    public void Constructor_NullCategoryId_DoesNotThrow()
    {
        var act = () => new Product(ValidOrgId, null, null, ValidName);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_NullUnitId_DoesNotThrow()
    {
        var act = () => new Product(ValidOrgId, ValidCategoryId, null, ValidName);
        act.Should().NotThrow();
    }

    // ── Constructor — guard clauses ──────────────────────────────────────────
    [Fact]
    public void Constructor_EmptyOrganizationId_Throws()
    {
        var act = () => new Product(Guid.Empty, null, null, ValidName);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_NullName_Throws()
    {
        var act = () => new Product(ValidOrgId, null, null, null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyName_Throws()
    {
        var act = () => new Product(ValidOrgId, null, null, "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WhitespaceName_Throws()
    {
        var act = () => new Product(ValidOrgId, null, null, "   ");
        act.Should().Throw<ArgumentException>();
    }

    // ── UpdateInformation ────────────────────────────────────────────────────
    [Fact]
    public void UpdateInformation_ValidParams_UpdatesAllFields()
    {
        var product = CreateValid();
        var newCategory = Guid.NewGuid();
        var newUnit = Guid.NewGuid();

        product.UpdateInformation(newCategory, newUnit, "Brown Rice");

        product.CategoryId.Should().Be(newCategory);
        product.UnitId.Should().Be(newUnit);
        product.Name.Should().Be("Brown Rice");
    }

    [Fact]
    public void UpdateInformation_ValidName_IsTrimmed()
    {
        var product = CreateValid();
        product.UpdateInformation(null, null, "  Wheat  ");
        product.Name.Should().Be("Wheat");
    }

    [Fact]
    public void UpdateInformation_NullCategoryAndUnit_DoesNotThrow()
    {
        var product = CreateValid();
        var act = () => product.UpdateInformation(null, null, ValidName);
        act.Should().NotThrow();
    }

    [Fact]
    public void UpdateInformation_EmptyName_Throws()
    {
        var product = CreateValid();
        var act = () => product.UpdateInformation(null, null, "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInformation_WhitespaceName_Throws()
    {
        var product = CreateValid();
        var act = () => product.UpdateInformation(null, null, "   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInformation_SetsUpdatedAt()
    {
        var product = CreateValid();
        var before = DateTime.UtcNow;

        product.UpdateInformation(null, null, "New Name");

        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().BeOnOrAfter(before);
    }

    // ── ChangeStatus ─────────────────────────────────────────────────────────
    [Fact]
    public void ChangeStatus_SetInactive_StatusIsInactive()
    {
        var product = CreateValid();
        product.ChangeStatus(ProductStatus.Inactive);
        product.Status.Should().Be(ProductStatus.Inactive);
    }

    [Fact]
    public void ChangeStatus_SetActive_StatusIsActive()
    {
        var product = CreateValid();
        product.ChangeStatus(ProductStatus.Inactive); // deactivate first
        product.ChangeStatus(ProductStatus.Active);
        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void ChangeStatus_SetsUpdatedAt()
    {
        var product = CreateValid();
        var before = DateTime.UtcNow;

        product.ChangeStatus(ProductStatus.Inactive);

        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().BeOnOrAfter(before);
    }
}


