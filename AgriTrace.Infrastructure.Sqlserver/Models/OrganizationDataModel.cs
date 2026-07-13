using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class OrganizationDataModel
    : BaseDataModel
{

    public Guid OrganizationTypeId { get; set; }


    public string Name { get; set; }


    public string? Address { get; set; }


    public OrganizationStatus Status { get; set; }

    // Navigation

    public OrganizationTypeDataModel OrganizationType { get; set; }



    public ICollection<UserDataModel> Users { get; set; }
        = new List<UserDataModel>();


    public ICollection<ProductDataModel> Products { get; set; }
        = new List<ProductDataModel>();


    public ICollection<BatchDataModel> Batches { get; set; }
        = new List<BatchDataModel>();

}