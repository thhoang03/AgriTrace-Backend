namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class CategoryDataModel : BaseDataModel
{

    public string Name { get; set; } = string.Empty;


    public string? Description { get; set; }



    // Navigation


    public ICollection<ProductDataModel> Products { get; set; }
        = new List<ProductDataModel>();

}