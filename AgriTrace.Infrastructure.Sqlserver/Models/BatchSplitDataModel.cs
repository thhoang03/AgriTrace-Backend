namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class BatchSplitDataModel : BaseDataModel
{

    // Batch gốc bị chia

    public Guid SourceBatchId { get; set; }



    // Navigation

    public BatchDataModel SourceBatch { get; set; } = null!;



    public ICollection<BatchSplitDetailDataModel> Details { get; set; }
        = new List<BatchSplitDetailDataModel>();

}