using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;


namespace AgriTrace.Domain.Entities;


public class Batch : BaseEntity
{

    public Guid ProductId { get; private set; }


    public string BatchCode { get; private set; } = null!;


    public decimal Quantity { get; private set; }


    public Guid UnitId { get; private set; }


    public DateTime ProductionDate { get; private set; }


    public DateTime? ExpiryDate { get; private set; }


    public BatchStatus Status { get; private set; }



    // Navigation

    public Product Product { get; private set; } = null!;


    public Unit Unit { get; private set; } = null!;




    private readonly List<SupplyChainEvent> _events = new();

    public IReadOnlyCollection<SupplyChainEvent> SupplyChainEvents
        => _events.AsReadOnly();



    private readonly List<QualityInspection> _inspections = new();

    public IReadOnlyCollection<QualityInspection> QualityInspections
        => _inspections.AsReadOnly();



    private readonly List<Certificate> _certificates = new();

    public IReadOnlyCollection<Certificate> Certificates
        => _certificates.AsReadOnly();




    private Batch()
    {

    }





    public Batch(
        Guid productId,
        string batchCode,
        decimal quantity,
        Guid unitId,
        DateTime productionDate,
        DateTime? expiryDate)
    {

        Validate(
            productId,
            batchCode,
            quantity,
            unitId,
            productionDate,
            expiryDate);



        ProductId = productId;

        BatchCode = batchCode.Trim();

        Quantity = quantity;

        UnitId = unitId;

        ProductionDate = productionDate;

        ExpiryDate = expiryDate;


        Status = BatchStatus.Created;

    }





    public void ChangeStatus(
        BatchStatus status)
    {
        Status = status;

        MarkUpdated();
    }





    public void UpdateQuantity(
        decimal quantity)
    {

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero");


        Quantity = quantity;


        MarkUpdated();

    }





    public void CompleteProduction()
    {
        Status = BatchStatus.Transporting;
        MarkUpdated();
    }





    public void Recall()
    {
        Status = BatchStatus.Recalled;

        MarkUpdated();
    }





    private static void Validate(
        Guid productId,
        string batchCode,
        decimal quantity,
        Guid unitId,
        DateTime productionDate,
        DateTime? expiryDate)
    {

        if (productId == Guid.Empty)
            throw new ArgumentException(
                "Product is required.");



        if (unitId == Guid.Empty)
            throw new ArgumentException(
                "Unit is required.");



        if (string.IsNullOrWhiteSpace(batchCode))
            throw new ArgumentException(
                "Batch code is required.");



        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must greater than zero.");



        if (expiryDate.HasValue &&
           expiryDate < productionDate)
        {
            throw new ArgumentException(
                "Expiry date invalid.");
        }

    }

}