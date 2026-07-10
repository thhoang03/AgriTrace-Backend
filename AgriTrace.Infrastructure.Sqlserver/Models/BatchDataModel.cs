namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class BatchDataModel : BaseDataModel
{

    public Guid ProductId { get; set; }


    public Guid CurrentOrganizationId { get; set; }


    public Guid UnitId { get; set; }



    public string BatchCode { get; set; } = string.Empty;



    public DateTime ProductionDate { get; set; }



    public DateTime? ExpiryDate { get; set; }



    public decimal Quantity { get; set; }



    public decimal RemainingQuantity { get; set; }



    public int Status { get; set; }



    public string? QRCode { get; set; }




    public Guid? ParentBatchId { get; set; }


    public Guid? RootBatchId { get; set; }




    // Navigation


    public ProductDataModel Product { get; set; } = null!;


    public OrganizationDataModel CurrentOrganization { get; set; } = null!;


    public UnitDataModel Unit { get; set; } = null!;



    public BatchDataModel? ParentBatch { get; set; }


    public ICollection<BatchDataModel> ChildBatches { get; set; }
        = new List<BatchDataModel>();


    public ICollection<SupplyChainEventDataModel> Events { get; set; }
        = new List<SupplyChainEventDataModel>();


    public ICollection<QualityInspectionDataModel> Inspections { get; set; }
        = new List<QualityInspectionDataModel>();


    public ICollection<CertificateDataModel> Certificates { get; set; }
        = new List<CertificateDataModel>();

}