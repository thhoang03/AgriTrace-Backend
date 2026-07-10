namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class NotificationDataModel : BaseDataModel
{

    // FK User

    public Guid UserId { get; set; }



    public string Title { get; set; } = string.Empty;



    public string Message { get; set; } = string.Empty;



    public bool IsRead { get; set; }




    // Navigation


    public UserDataModel User { get; set; } = null!;

}