namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class SupplyChainEventDataModel
    : BaseDataModel
{

    public Guid BatchId { get; set; }


    public Guid EventTypeId { get; set; }


    public Guid OrganizationId { get; set; }


    public Guid PerformedByUserId { get; set; }



    public string? EventData { get; set; }


    public string? Location { get; set; }


    public string? PreviousHash { get; set; }


    public string? CurrentHash { get; set; }



    public DateTime EventTime { get; set; }




    // Navigation


    public BatchDataModel Batch { get; set; } = null!;


    public EventTypeDataModel EventType { get; set; } = null!;


    public OrganizationDataModel Organization { get; set; } = null!;


    public UserDataModel PerformedByUser { get; set; } = null!;

}