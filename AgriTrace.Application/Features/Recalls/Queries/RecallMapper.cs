using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;
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
