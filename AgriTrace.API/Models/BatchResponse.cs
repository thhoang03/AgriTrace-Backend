using AgriTrace.Domain.Common.Enums;


namespace AgriTrace.API.Models;


public sealed class BatchResponse
{

    public Guid Id { get; set; }


    public Guid ProductId { get; set; }


    public Guid UnitId { get; set; }


    public string BatchCode { get; set; } = null!;


    public decimal Quantity { get; set; }


    public decimal RemainingQuantity { get; set; }


    public DateTime ProductionDate { get; set; }


    public DateTime? ExpiryDate { get; set; }


    public string Status { get; set; } = null!;

}