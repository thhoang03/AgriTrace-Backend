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

public class OrganizationTests
{
    private static readonly Guid ValidTypeId = Guid.NewGuid();

    private static Organization CreateValid(string name = "AgriCo Farms")
        => new(ValidTypeId, name, "123 Main Street");

    // ── Constructor — happy path ─────────────────────────────────────────────
    [Fact]
    public void Constructor_ValidParams_SetsProperties()
    {
        var org = CreateValid("My Farm");

        org.OrganizationTypeId.Should().Be(ValidTypeId);
        org.Name.Should().Be("My Farm");
        org.Address.Should().Be("123 Main Street");
        org.Status.Should().Be(OrganizationStatus.Active);
    }

    [Fact]
    public void Constructor_Name_IsTrimmed()
    {
        var org = new Organization(ValidTypeId, "  Farm Co  ", null);
        org.Name.Should().Be("Farm Co");
    }

    [Fact]
    public void Constructor_NullAddress_IsAllowed()
    {
        var act = () => new Organization(ValidTypeId, "Name", null);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_NewOrg_StatusIsActive()
    {
        var org = CreateValid();
        org.Status.Should().Be(OrganizationStatus.Active);
    }

    // ── Constructor — guard clauses ──────────────────────────────────────────
    [Fact]
    public void Constructor_EmptyOrganizationTypeId_Throws()
    {
        var act = () => new Organization(Guid.Empty, "Name", null);
        act.Should().Throw<ArgumentException>().WithMessage("*Organization type*");
    }

    [Fact]
    public void Constructor_EmptyName_Throws()
    {
        var act = () => new Organization(ValidTypeId, "   ", null);
        act.Should().Throw<ArgumentException>().WithMessage("*name*");
    }

    [Fact]
    public void Constructor_NullName_Throws()
    {
        var act = () => new Organization(ValidTypeId, null!, null);
        act.Should().Throw<ArgumentException>();
    }

    // ── UpdateInformation ────────────────────────────────────────────────────
    [Fact]
    public void UpdateInformation_ValidParams_UpdatesFields()
    {
        var org = CreateValid();
        var newTypeId = Guid.NewGuid();

        org.UpdateInformation(newTypeId, "New Name", "New Address");

        org.OrganizationTypeId.Should().Be(newTypeId);
        org.Name.Should().Be("New Name");
        org.Address.Should().Be("New Address");
    }

    [Fact]
    public void UpdateInformation_EmptyName_Throws()
    {
        var org = CreateValid();
        var act = () => org.UpdateInformation(ValidTypeId, "   ", null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInformation_EmptyTypeId_Throws()
    {
        var org = CreateValid();
        var act = () => org.UpdateInformation(Guid.Empty, "Name", null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInformation_SetsUpdatedAt()
    {
        var org = CreateValid();
        var before = DateTime.UtcNow;
        org.UpdateInformation(ValidTypeId, "New Name", null);
        org.UpdatedAt.Should().NotBeNull().And.BeOnOrAfter(before);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────
    [Fact]
    public void Deactivate_SetsStatusInactive()
    {
        var org = CreateValid();
        org.Deactivate();
        org.Status.Should().Be(OrganizationStatus.Inactive);
    }

    [Fact]
    public void Activate_AfterDeactivate_SetsStatusActive()
    {
        var org = CreateValid();
        org.Deactivate();
        org.Activate();
        org.Status.Should().Be(OrganizationStatus.Active);
    }

    [Fact]
    public void Deactivate_SetsUpdatedAt()
    {
        var org = CreateValid();
        var before = DateTime.UtcNow;
        org.Deactivate();
        org.UpdatedAt.Should().NotBeNull().And.BeOnOrAfter(before);
    }
}

