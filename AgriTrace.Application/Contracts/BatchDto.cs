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

    // ---- Fields aligned to swagger BatchListItem / BatchDetail ----
    // The following joined fields are not carried by the Batch domain entity and cannot be
    // populated from the current read projection (Mapster Adapt<BatchDto> from the rehydrated
    // entity). They are left null until the read layer projects joined data (Phase 11 follow-up).
    public string? ProductName { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? UnitCode { get; set; }

    public Guid CurrentOrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public string? QrCodeUrl { get; set; }
}
