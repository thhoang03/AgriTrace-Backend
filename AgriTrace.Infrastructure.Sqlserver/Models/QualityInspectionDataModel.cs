using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.QualityInspections;

namespace AgriTrace.Infrastructure.Sqlserver.Models;

public class QualityInspectionDataModel : BaseDataModel
{
    public Guid BatchId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? InspectorId { get; set; }

    public InspectionType InspectionType { get; set; }

    public InspectionStatus Status { get; set; }

    public string? OverallResult { get; set; }

    public DateTime InspectionDate { get; set; }

    public string? Notes { get; set; }

    // Navigation

    public BatchDataModel Batch { get; set; } = null!;

    public UserDataModel? Inspector { get; set; }

    public ICollection<InspectionLabTestDataModel> LabTests { get; set; }
        = new List<InspectionLabTestDataModel>();
}

