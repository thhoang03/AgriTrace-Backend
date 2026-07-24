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


public class BatchDataModel : BaseDataModel
{

    // =========================
    // Basic Information
    // =========================


    public Guid ProductId { get; set; }


    public Guid CurrentOrganizationId { get; set; }


    public Guid UnitId { get; set; }



    public string BatchCode { get; set; } = string.Empty;



    public string? QRCode { get; set; }



    public DateTime ProductionDate { get; set; }



    public DateTime? ExpiryDate { get; set; }



    public decimal Quantity { get; set; }



    public decimal RemainingQuantity { get; set; }



    public decimal SourceQuantity { get; set; }



    public BatchStatus Status { get; set; }







    // =========================
    // Traceability
    // =========================


    public Guid? ParentBatchId { get; set; }



    public Guid? RootBatchId { get; set; }



    public Guid? SplitId { get; set; }








    // =========================
    // Navigation
    // =========================


    public ProductDataModel Product { get; set; } = null!;



    public OrganizationDataModel CurrentOrganization { get; set; } = null!;



    public UnitDataModel Unit { get; set; } = null!;





    // Parent - Child Batch


    public BatchDataModel? ParentBatch { get; set; }



    public ICollection<BatchDataModel> ChildBatches { get; set; }
        = new List<BatchDataModel>();








    // Supply Chain Events


    public ICollection<SupplyChainEventDataModel> Events { get; set; }
        = new List<SupplyChainEventDataModel>();







    // Quality Inspection


    public ICollection<QualityInspectionDataModel> QualityInspections { get; set; }
        = new List<QualityInspectionDataModel>();







    // Certificates


    public ICollection<CertificateDataModel> Certificates { get; set; }
        = new List<CertificateDataModel>();







    // Recall


    public ICollection<RecallDataModel> Recalls { get; set; }
        = new List<RecallDataModel>();

}
