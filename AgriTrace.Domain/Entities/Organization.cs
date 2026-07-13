using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;

public class Organization : BaseEntity
{

    public Guid OrganizationTypeId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Address { get; private set; }

    public OrganizationStatus Status { get; private set; }



    private Organization()
    {

    }



    public Organization(
        Guid organizationTypeId,
        string name,
        string? address)
    {

        Validate(
            organizationTypeId,
            name);


        OrganizationTypeId = organizationTypeId;

        Name = name.Trim();

        Address = address?.Trim();


        Status = OrganizationStatus.Active;

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
    public void SetStatus(
        OrganizationStatus status)
    {
        Status = status;
    }

    private static void Validate(
        Guid organizationTypeId,
        string name)
    {

        if (organizationTypeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization type is required.");
        }


        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required.");
        }

    }

}