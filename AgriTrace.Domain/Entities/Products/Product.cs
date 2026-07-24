using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities.Products;

public class Product : BaseEntity
{
    public Guid OrganizationId { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? UnitId { get; private set; }

    public string Name { get; private set; } = null!;

    // Navigation

    public Organization Organization { get; private set; } = null!;

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
        Status = ProductStatus.Created;
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
        Unit? unit,
        Organization? organization = null,
        ProductStatus status = ProductStatus.Active)
        : base(id, createdAt, updatedAt)
    {
        OrganizationId = organizationId;
        CategoryId = categoryId;
        UnitId = unitId;
        Name = name;
        Category = category;
        Unit = unit;
        Organization = organization;
        Status = status;
    }

    public void UpdateInformation(
        Guid? categoryId,
        Guid? unitId,
        string name,
        Guid? organizationId = null)
    {
        var targetOrgId = (organizationId.HasValue && organizationId.Value != Guid.Empty)
            ? organizationId.Value
            : OrganizationId;

        Validate(
            targetOrgId,
            name
        );

        OrganizationId = targetOrgId;
        CategoryId = categoryId;
        UnitId = unitId;
        Name = name.Trim();

        MarkUpdated();
    }

    public ProductStatus Status { get; private set; } = ProductStatus.Created;

    public void ChangeStatus(ProductStatus status)
    {
        Status = status;
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
