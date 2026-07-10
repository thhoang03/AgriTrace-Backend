namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class BatchMergeDataModel : BaseDataModel
{

    // Batch mới sau khi merge

    public Guid NewBatchId { get; set; }




    // Navigation


    public BatchDataModel NewBatch { get; set; } = null!;



    public ICollection<BatchMergeSourceDataModel> Sources { get; set; }
        = new List<BatchMergeSourceDataModel>();

}