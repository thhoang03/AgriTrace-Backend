using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Product : BaseEntity
{
    public Guid OrganizationId { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? UnitId { get; private set; }

    public string Name { get; private set; }

    // Navigation

    public Organization Organization { get; private set; }

    public Category? Category { get; private set; }

    public Unit? Unit { get; private set; }

    private readonly List<Batch> _batches = new();

    public IReadOnlyCollection<Batch> Batches => _batches.AsReadOnly();

    private Product()
    {

    }

    public Product(
        Guid organizationId,
        Guid? categoryId,
        Guid? unitId,
        string name)
    {
        Validate(
            organizationId,
            name
        );

        OrganizationId = organizationId;
        CategoryId = categoryId;
        UnitId = unitId;
        Name = name.Trim();
    }

    public Product(
        Guid id,
        Guid organizationId,
        Guid? categoryId,
        Guid? unitId,
        string name,
        DateTime createdAt,
        DateTime? updatedAt,
        Category? category,
        Unit? unit)
        : base(id, createdAt, updatedAt)
    {
        OrganizationId = organizationId;
        CategoryId = categoryId;
        UnitId = unitId;
        Name = name;
        Category = category;
        Unit = unit;
    }

    public void UpdateInformation(
        Guid? categoryId,
        Guid? unitId,
        string name)
    {
        Validate(
            OrganizationId,
            name
        );

        CategoryId = categoryId;
        UnitId = unitId;
        Name = name.Trim();

        MarkUpdated();
    }

    // NOTE: The Product domain entity does not yet persist an IsActive flag (no DB column / migration
    // in this phase). ChangeStatus toggles the in-memory field; persistence follows in a later phase
    // (Phase 11 follow-up) once the column and mapping are added.
    public bool IsActive { get; private set; } = true;

    public void ChangeStatus(bool isActive)
    {
        IsActive = isActive;
        MarkUpdated();
    }

    private static void Validate(
        Guid organizationId,
        string name)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.");
        }
    }

    internal void UpdateProduct(Guid? categoryId, Guid? unitId, string name)
    {
        throw new NotImplementedException();
    }
}