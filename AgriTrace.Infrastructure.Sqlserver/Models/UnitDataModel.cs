using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class UnitDataModel : BaseDataModel
{

    public string Code { get; set; } = string.Empty;


    public string Name { get; set; } = string.Empty;


    public string? Description { get; set; }


    public string? Symbol { get; set; }


    public UnitCategory Category { get; set; } = UnitCategory.Weight;


    public decimal? ConversionToBase { get; set; }



    // Navigation


    public ICollection<ProductDataModel> Products { get; set; }
        = new List<ProductDataModel>();

}