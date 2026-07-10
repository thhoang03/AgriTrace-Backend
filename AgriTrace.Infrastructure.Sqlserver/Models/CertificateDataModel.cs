namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class CertificateDataModel : BaseDataModel
{

    // FK Batch

    public Guid BatchId { get; set; }



    // FK QualityInspection

    public Guid? InspectionId { get; set; }



    public string? CertificateType { get; set; }



    public string? FileUrl { get; set; }



    public DateTime? IssuedDate { get; set; }




    // Navigation


    public BatchDataModel Batch { get; set; } = null!;



    public QualityInspectionDataModel? Inspection { get; set; }

}