using AgriTrace.Domain.Entities.QualityInspections;

namespace AgriTrace.Infrastructure.Sqlserver.Models;

public class InspectionLabTestDataModel : BaseDataModel
{
    public Guid InspectionId { get; set; }

    public string TestName { get; set; } = string.Empty;

    public string? MeasuredValue { get; set; }

    public string? Unit { get; set; }

    public string? MinStandardValue { get; set; }

    public string? MaxStandardValue { get; set; }

    public bool IsPassed { get; set; }

    public string? Remark { get; set; }

    public QualityInspectionDataModel Inspection { get; set; } = null!;
}
