namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class ProductDataModel : BaseDataModel
{

    public Guid OrganizationId { get; set; }


    public Guid? CategoryId { get; set; }


    public Guid? UnitId { get; set; }



    public string Name { get; set; } = string.Empty;



    // Navigation


    public OrganizationDataModel Organization { get; set; } = null!;


    public CategoryDataModel? Category { get; set; }


    public UnitDataModel? Unit { get; set; }



    public ICollection<BatchDataModel> Batches { get; set; }
        = new List<BatchDataModel>();

}