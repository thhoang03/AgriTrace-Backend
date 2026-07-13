namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class EventTypeDataModel : BaseDataModel
{

    public string Code { get; set; } = string.Empty;


    public string Name { get; set; } = string.Empty;



    // Navigation


    public ICollection<SupplyChainEventDataModel> Events { get; set; }
        = new List<SupplyChainEventDataModel>();

}