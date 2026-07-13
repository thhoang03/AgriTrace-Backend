using AgriTrace.Domain.Common.Enums;
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