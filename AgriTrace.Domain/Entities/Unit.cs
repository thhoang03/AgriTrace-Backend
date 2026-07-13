using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Unit : BaseEntity
{
    public string Code { get; private set; }

    public string Name { get; private set; }

    private readonly List<Product> _products = new();

    public IReadOnlyCollection<Product> Products =>
        _products.AsReadOnly();

    private Unit()
    {

    }

    public Unit(
        string code,
        string name)
    {
        Validate(code, name);

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
    }

    public void UpdateInformation(
        string code,
        string name)
    {
        Validate(code, name);

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();

        MarkUpdated();
    }

    private static void Validate(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Unit code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit name is required.");
        }
    }
}