using AgriTrace.API.Models;
using AgriTrace.API.Models.Batches;
using AgriTrace.Application.Features.Batches.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Thao tác tách (split) và gộp (merge) Batch.
/// </summary>
[ApiController]
[Authorize]
public sealed class BatchSplitMergeController : ControllerBase
{
    private readonly ISender _sender;

    public BatchSplitMergeController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tách Batch
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/split")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Split(
        Guid batchId,
        [FromBody] SplitBatchRequest request,
        CancellationToken cancellationToken)
    {
        var splits = request.Splits
            .Select(s => new SplitDetailInput(s.Quantity, s.UnitId))
            .ToList();

        var result = await _sender.Send(
            new SplitBatchCommand(batchId, splits, Guid.Empty),
            cancellationToken);

        var data = new SplitBatchResponse
        {
            ParentBatchId = result.ParentBatchId,
            ChildBatchIds = result.ChildBatchIds
        };

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.Success(data, "Tách lô thành công"));
    }

    /// <summary>
    /// Gộp Batch
    /// </summary>
    [HttpPost("api/v1/batches/merge")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Merge(
        [FromBody] MergeBatchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MergeBatchCommand(
                request.SourceBatchIds,
                request.ProductId,
                request.Quantity,
                request.UnitId,
                request.ProductionDate),
            cancellationToken);

        var data = new MergeBatchResponse
        {
            NewBatchId = result.NewBatchId,
            BatchCode = result.BatchCode
        };

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.Success(data, "Gộp lô thành công"));
    }
}
