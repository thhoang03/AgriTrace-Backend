namespace AgriTrace.Infrastructure.Sqlserver.Models;


public class BatchMergeSourceDataModel
{

    public Guid BatchMergeId { get; set; }



    public Guid SourceBatchId { get; set; }



    public decimal Quantity { get; set; }




    // Navigation


    public BatchMergeDataModel BatchMerge { get; set; } = null!;



    public BatchDataModel SourceBatch { get; set; } = null!;

}