namespace AgriTrace.API.Models;

public sealed record BatchRequest(
    Guid ProductId,
    Guid UnitId,
    string BatchCode,
    decimal Quantity,
    DateTime ProductionDate,
    DateTime? ExpiryDate
);