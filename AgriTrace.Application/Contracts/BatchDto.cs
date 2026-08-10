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

namespace AgriTrace.Application.Contracts;

public sealed class BatchDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UnitId { get; set; }


    public string BatchCode { get; set; } = null!;


    public string? Gs1BatchCode { get; set; }


    public decimal Quantity { get; set; }


    public decimal RemainingQuantity { get; set; }


    public DateTime ProductionDate { get; set; }


    public DateTime? ExpiryDate { get; set; }


    public BatchStatus Status { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime? UpdatedAt { get; set; }

    // ---- Fields aligned to swagger BatchListItem / BatchDetail ----
    // entity). They are left null until the read layer projects joined data (Phase 11 follow-up).
    public string? ProductName { get; set; }

    public string? ProductGtin { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? UnitCode { get; set; }

    public Guid CurrentOrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public string? QrCodeUrl { get; set; }

    public string? FarmerName { get; set; }

    public string? Location { get; set; }

    public string? ProductionArea { get; set; }
}

