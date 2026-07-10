namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class UnitDataModel : BaseDataModel
{

    public string Code { get; set; } = string.Empty;


    public string Name { get; set; } = string.Empty;



    // Navigation


    public ICollection<ProductDataModel> Products { get; set; }
        = new List<ProductDataModel>();

}