namespace AgriTrace.Infrastructure.Sqlserver.Models;

public class OrganizationTypeDataModel : BaseDataModel
{
    public string Code { get; set; } = string.Empty;


    public string Name { get; set; } = string.Empty;


    public string? Description { get; set; }


    // Navigation

    public ICollection<OrganizationDataModel> Organizations { get; set; }
        = new List<OrganizationDataModel>();
}