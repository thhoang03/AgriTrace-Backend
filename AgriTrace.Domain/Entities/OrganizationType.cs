using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class OrganizationType : BaseEntity
{
    public string Code { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    private readonly List<Organization> _organizations = new();

    public IReadOnlyCollection<Organization> Organizations
        => _organizations.AsReadOnly();

    private OrganizationType()
    {
    }

    public OrganizationType(
        string code,
        string name,
        string? description)
    {
        Validate(code, name);

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();
    }

    public OrganizationType(
        Guid id,
        string code,
        string name,
        string? description,
        DateTime createdAt,
        DateTime? updatedAt)
        : base(id, createdAt, updatedAt)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();
    }

    public void UpdateInformation(
        string code,
        string name,
        string? description)
    {
        Validate(code, name);

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();

        MarkUpdated();
    }

    private static void Validate(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Organization type code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Organization type name is required.");
        }
    }
}