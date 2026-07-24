using AgriTrace.Domain.Common;
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

public class Organization : BaseEntity
{
    public Guid OrganizationTypeId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Address { get; private set; }

    public OrganizationStatus Status { get; private set; }

    public OrganizationType? OrganizationType { get; private set; }

    private Organization() { }

    public Organization(
        Guid organizationTypeId,
        string name,
        string? address)
    {
        Validate(organizationTypeId, name);

        OrganizationTypeId = organizationTypeId;
        Name = name.Trim();
        Address = address?.Trim();
        Status = OrganizationStatus.Active;
    }

    // Dùng khi dựng lại (rehydrate) Organization từ dữ liệu đã có trong database,
    // giữ nguyên Id, Status và các mốc thời gian gốc — không validate lại vì
    // dữ liệu được coi là đã hợp lệ từ trước khi lưu.
    public Organization(
        Guid id,
        Guid organizationTypeId,
        string name,
        string? address,
        OrganizationStatus status,
        DateTime createdAt,
        DateTime? updatedAt,
        OrganizationType? organizationType)
    {
        Id = id;
        OrganizationTypeId = organizationTypeId;
        Name = name.Trim();
        Address = address?.Trim();
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        OrganizationType = organizationType;
    }

    public void UpdateInformation(
        Guid organizationTypeId,
        string name,
        string? address)
    {
        Validate(organizationTypeId, name);

        OrganizationTypeId = organizationTypeId;
        Name = name.Trim();
        Address = address?.Trim();

        MarkUpdated();
    }

    public void Activate()
    {
        Status = OrganizationStatus.Active;
        MarkUpdated();
    }

    public void Deactivate()
    {
        Status = OrganizationStatus.Inactive;
        MarkUpdated();
    }

    // Infrastructure mapping
    public void SetStatus(OrganizationStatus status)
    {
        Status = status;
    }

    private static void Validate(Guid organizationTypeId, string name)
    {
        if (organizationTypeId == Guid.Empty)
            throw new ArgumentException("Organization type is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.");
    }
}

