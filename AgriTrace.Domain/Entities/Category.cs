using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<Product> _products = new();

    public IReadOnlyCollection<Product> Products =>
        _products.AsReadOnly();

    private Category()
    {
    }

    public Category(
        string name,
        string? description)
    {
        Validate(name);

        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
    }

    public Category(
        Guid id,
        string name,
        string? description,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isActive)
    {
        Id = id;
        Name = name.Trim();
        Description = description?.Trim();
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsActive = isActive;
    }

    public void UpdateInformation(
        string name,
        string? description)
    {
        Validate(name);

        Name = name.Trim();
        Description = description?.Trim();

        MarkUpdated();
    }

    public void ChangeStatus(bool isActive)
    {
        IsActive = isActive;
        MarkUpdated();
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.");
        }
    }
}