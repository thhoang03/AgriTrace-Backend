using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

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

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.");
        }
    }
}