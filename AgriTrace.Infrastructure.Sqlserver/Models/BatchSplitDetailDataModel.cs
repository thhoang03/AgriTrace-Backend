namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class BatchSplitDetailDataModel : BaseDataModel
{

    // FK BatchSplit

    public Guid SplitId { get; set; }



    // Batch được tạo ra sau khi split

    public Guid TargetBatchId { get; set; }



    public decimal Quantity { get; set; }




    // Navigation


    public BatchSplitDataModel Split { get; set; } = null!;



    public BatchDataModel TargetBatch { get; set; } = null!;

}