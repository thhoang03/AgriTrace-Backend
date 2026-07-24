using AgriTrace.Domain.Common;
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


namespace AgriTrace.Domain.Entities.Batches;


public class Batch : BaseEntity
{

    // =========================
    // Basic Information
    // =========================

    public Guid ProductId { get; private set; }


    public string BatchCode { get; private set; } = null!;


    public decimal Quantity { get; private set; }


    public decimal RemainingQuantity { get; private set; }


    public Guid UnitId { get; private set; }



    public DateTime ProductionDate { get; private set; }


    public DateTime? ExpiryDate { get; private set; }



    public BatchStatus Status { get; private set; }




    // =========================
    // Organization
    // =========================

    public Guid CurrentOrganizationId { get; private set; }



    // =========================
    // Traceability
    // =========================

    public string? QRCode { get; private set; }



    public Guid? ParentBatchId { get; private set; }



    public Guid? RootBatchId { get; private set; }



    public Guid? SplitId { get; private set; }



    public decimal SourceQuantity { get; private set; }







    // =========================
    // Navigation
    // =========================


    public Product Product { get; private set; } = null!;


    public Unit Unit { get; private set; } = null!;


    public Organization? CurrentOrganization { get; private set; }


    public Batch? ParentBatch { get; private set; }



    private readonly List<Batch> _childBatches = new();


    public IReadOnlyCollection<Batch> ChildBatches
        => _childBatches.AsReadOnly();





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



    // =========================
    // Rehydration (from persistence)
    // =========================

    /// <summary>
    /// Reconstructs a Batch from persisted data.
    /// Bypasses business-constructor side-effects (new Id, validation, RootBatchId override).
    /// Use ONLY in repository ToEntity conversions.
    /// </summary>
    public static Batch Rehydrate(
        Guid id,
        Guid productId,
        string batchCode,
        decimal quantity,
        decimal remainingQuantity,
        decimal sourceQuantity,
        Guid unitId,
        DateTime productionDate,
        DateTime? expiryDate,
        BatchStatus status,
        Guid currentOrganizationId,
        string? qrCode,
        Guid? parentBatchId,
        Guid? rootBatchId,
        Guid? splitId,
        DateTime createdAt,
        DateTime? updatedAt,
        Product? product = null,
        Unit? unit = null,
        Organization? currentOrganization = null)
    {
        var batch = new Batch();

        batch.Id = id;
        batch.ProductId = productId;
        batch.BatchCode = batchCode;
        batch.Quantity = quantity;
        batch.RemainingQuantity = remainingQuantity;
        batch.SourceQuantity = sourceQuantity;
        batch.UnitId = unitId;
        batch.ProductionDate = productionDate;
        batch.ExpiryDate = expiryDate;
        batch.Status = status;
        batch.CurrentOrganizationId = currentOrganizationId;
        batch.QRCode = qrCode;
        batch.ParentBatchId = parentBatchId;
        batch.RootBatchId = rootBatchId;
        batch.SplitId = splitId;
        batch.CreatedAt = createdAt;
        batch.UpdatedAt = updatedAt;

        if (product != null) batch.Product = product;
        if (unit != null) batch.Unit = unit;
        if (currentOrganization != null) batch.CurrentOrganization = currentOrganization;

        return batch;
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



        Id = Guid.NewGuid();



        ProductId =
            productId;



        BatchCode =
            batchCode.Trim();



        Quantity =
            quantity;



        RemainingQuantity =
            quantity;



        SourceQuantity =
            quantity;



        UnitId =
            unitId;



        ProductionDate =
            productionDate;



        ExpiryDate =
            expiryDate;



        Status =
            BatchStatus.Created;



        RootBatchId =
            Id;


    }









    // =========================
    // Update
    // =========================


    public void UpdateInformation(
        string batchCode,
        decimal quantity,
        DateTime productionDate,
        DateTime? expiryDate)
    {

        Validate(
            ProductId,
            batchCode,
            quantity,
            UnitId,
            productionDate,
            expiryDate);



        BatchCode =
            batchCode.Trim();



        if (Quantity == RemainingQuantity)
        {
            RemainingQuantity =
                quantity;
        }



        Quantity =
            quantity;



        ProductionDate =
            productionDate;



        ExpiryDate =
            expiryDate;



        MarkUpdated();

    }









    public void UpdateQuantity(
        decimal quantity)
    {

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }



        if (Quantity == RemainingQuantity)
        {
            RemainingQuantity =
                quantity;
        }



        Quantity =
            quantity;



        MarkUpdated();

    }









    // =========================
    // Organization
    // =========================


    public void ChangeOrganization(
        Guid organizationId)
    {

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization is required.");
        }



        CurrentOrganizationId =
            organizationId;



        MarkUpdated();

    }









    public void SetQRCode(
        string qrCode)
    {

        QRCode =
            qrCode;


        MarkUpdated();

    }









    // =========================
    // Inventory
    // =========================


    public void ReduceRemainingQuantity(
        decimal quantity)
    {

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }



        if (quantity > RemainingQuantity)
        {
            throw new InvalidOperationException(
                "Remaining quantity is not enough.");
        }



        RemainingQuantity -= quantity;



        MarkUpdated();

    }









    public void RestoreRemainingQuantity(
        decimal quantity)
    {

        if (quantity < 0)
        {
            throw new ArgumentException(
                "Quantity cannot be negative.");
        }



        RemainingQuantity =
            quantity;



        MarkUpdated();

    }









    // =========================
    // Split Batch
    // =========================


    public Batch CreateChildBatch(
        string batchCode,
        decimal quantity,
        Guid splitId)
    {

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }



        ReduceRemainingQuantity(
            quantity);



        var child =
            new Batch(
                ProductId,
                batchCode,
                quantity,
                UnitId,
                ProductionDate,
                ExpiryDate);



        child.ParentBatchId =
            Id;



        child.RootBatchId =
            RootBatchId ?? Id;



        child.SplitId =
            splitId;



        child.SourceQuantity =
            quantity;



        child.CurrentOrganizationId =
            CurrentOrganizationId;



        return child;

    }









    public void SetTraceabilityInfo(
        Guid? parentBatchId,
        Guid? rootBatchId,
        Guid? splitId,
        decimal sourceQuantity)
    {

        ParentBatchId =
            parentBatchId;



        RootBatchId =
            rootBatchId;



        SplitId =
            splitId;



        SourceQuantity =
            sourceQuantity;



        MarkUpdated();

    }









    // =========================
    // Status
    // =========================


    public void ChangeStatus(
        BatchStatus status)
    {

        Status =
            status;



        MarkUpdated();

    }









    public void CompleteProduction()
    {

        Status =
            BatchStatus.Transporting;



        MarkUpdated();

    }









    public void Recall()
    {

        Status =
            BatchStatus.Recalled;



        MarkUpdated();

    }

    public bool CanBeRecalled()
    {
        return Status != BatchStatus.Recalled;
    }











    // =========================
    // Validation
    // =========================


    private static void Validate(
        Guid productId,
        string batchCode,
        decimal quantity,
        Guid unitId,
        DateTime productionDate,
        DateTime? expiryDate)
    {


        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product is required.");
        }




        if (unitId == Guid.Empty)
        {
            throw new ArgumentException(
                "Unit is required.");
        }




        if (string.IsNullOrWhiteSpace(batchCode))
        {
            throw new ArgumentException(
                "Batch code is required.");
        }




        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must greater than zero.");
        }




        if (expiryDate.HasValue &&
           expiryDate < productionDate)
        {
            throw new ArgumentException(
                "Expiry date invalid.");
        }

    }

}

