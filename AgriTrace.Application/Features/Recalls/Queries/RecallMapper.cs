using AgriTrace.Application.Contracts;
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
using AgriTrace.Domain.Interfaces.Inbound;

namespace AgriTrace.Application.Features.Recalls.Queries;

/// <summary>
/// Maps a <see cref="Recall"/> domain entity to a <see cref="RecallDto"/>, resolving the batch code
/// and creator name via the relevant services.
/// </summary>
public static class RecallMapper
{
    public static async Task<RecallDto> ToDtoAsync(
        Recall recall,
        IBatchReadService batchReadService,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var batch = await batchReadService.GetByIdAsync(recall.BatchId, cancellationToken);
        var user = await userService.GetByIdAsync(recall.CreatedBy, cancellationToken);

        return new RecallDto
        {
            RecallId = recall.Id,
            BatchId = recall.BatchId,
            BatchCode = batch?.BatchCode,
            ProductId = batch?.ProductId,
            ProductName = batch?.Product?.Name,
            OrganizationId = batch?.CurrentOrganizationId,
            OrganizationName = batch?.CurrentOrganization?.Name ?? batch?.Product?.Organization?.Name,
            CreatedBy = recall.CreatedBy,
            CreatedByName = user?.FullName,
            Reason = recall.Reason,
            Severity = (int)recall.Severity,
            SeverityName = recall.Severity.ToString().ToUpperInvariant(),
            Status = (int)recall.Status,
            StatusName = recall.Status.ToString().ToUpperInvariant(),
            CreatedAt = recall.CreatedAt
        };
    }
}

