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


public class UserDataModel : BaseDataModel
{

    public Guid? OrganizationId { get; set; }


    public string FullName { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;


    public string? PasswordHash { get; set; }


    public string? Phone { get; set; }


    public UserRole Role { get; set; }


    public bool IsActive { get; set; }


    public string? RefreshToken { get; set; }


    public DateTime? RefreshTokenExpiry { get; set; }


    public string? ResetPasswordToken { get; set; }


    public DateTime? ResetPasswordTokenExpiry { get; set; }


    // Navigation


    public OrganizationDataModel? Organization { get; set; }



    public ICollection<SupplyChainEventDataModel> Events { get; set; }
        = new List<SupplyChainEventDataModel>();


    public ICollection<RecallDataModel> Recalls { get; set; }
        = new List<RecallDataModel>();


    public ICollection<NotificationDataModel> Notifications { get; set; }
        = new List<NotificationDataModel>();

}
