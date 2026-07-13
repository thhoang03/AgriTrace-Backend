namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class UserDataModel : BaseDataModel
{

    public Guid? OrganizationId { get; set; }


    public string FullName { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;


    public string? PasswordHash { get; set; }


    public string Role { get; set; } = string.Empty;


    public bool IsActive { get; set; }


    // Navigation


    public OrganizationDataModel? Organization { get; set; }



    public ICollection<SupplyChainEventDataModel> Events { get; set; }
        = new List<SupplyChainEventDataModel>();


    public ICollection<RecallDataModel> Recalls { get; set; }
        = new List<RecallDataModel>();


    public ICollection<NotificationDataModel> Notifications { get; set; }
        = new List<NotificationDataModel>();

}