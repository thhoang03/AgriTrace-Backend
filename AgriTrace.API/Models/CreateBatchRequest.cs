using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for creating a new batch. Matches swagger <c>CreateBatchRequest</c>.
/// The server generates the batch code; the client does not provide it.
/// </summary>
public sealed record CreateBatchRequest(
    [Required] Guid ProductId,
    [Required] decimal Quantity,
    [Required] Guid UnitId,
    [Required] DateOnly ProductionDate,
    DateOnly? ExpiryDate
);
