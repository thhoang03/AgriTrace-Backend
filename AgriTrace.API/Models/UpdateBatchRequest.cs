namespace AgriTrace.API.Models;

/// <summary>
/// Request body for updating an existing batch. Matches swagger <c>UpdateBatchRequest</c>.
/// Only <c>quantity</c> and <c>expiryDate</c> are updatable.
/// </summary>
public sealed record UpdateBatchRequest(
    decimal? Quantity,
    DateOnly? ExpiryDate
);
