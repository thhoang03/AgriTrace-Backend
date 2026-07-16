using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Application.Contracts;

public sealed class BatchDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UnitId { get; set; }


    public string BatchCode { get; set; } = null!;


    public decimal Quantity { get; set; }


    public decimal RemainingQuantity { get; set; }


    public DateTime ProductionDate { get; set; }


    public DateTime? ExpiryDate { get; set; }


    public BatchStatus Status { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime? UpdatedAt { get; set; }
}