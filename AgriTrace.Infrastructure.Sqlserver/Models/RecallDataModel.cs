namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class RecallDataModel : BaseDataModel
{

    // FK Batch

    public Guid BatchId { get; set; }



    // FK User tạo Recall

    public Guid CreatedBy { get; set; }



    public string? Reason { get; set; }



    public int Severity { get; set; }



    public int Status { get; set; }




    // Navigation


    public BatchDataModel Batch { get; set; } = null!;



    public UserDataModel Creator { get; set; } = null!;

    public ICollection<RecallDataModel> Recalls { get; set; }

}