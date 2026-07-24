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
