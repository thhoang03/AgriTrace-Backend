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
using AgriTrace.Infrastructure.Sqlserver.Models;

namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class QualityInspectionDataModel : BaseDataModel
{

    // FK Batch

    public Guid BatchId { get; set; }



    // FK Inspector(User)

    public Guid InspectorId { get; set; }



    public InspectionStatus Status { get; set; }



    public string? Result { get; set; }



    public string? Notes { get; set; }



    // Navigation


    public BatchDataModel Batch { get; set; } = null!;


    public UserDataModel Inspector { get; set; } = null!;

    public ICollection<CertificateDataModel> Certificates { get; set; }
    = new List<CertificateDataModel>();

}
