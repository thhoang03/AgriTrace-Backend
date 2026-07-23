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
