using AgriTrace.API.Models;
using AgriTrace.API.Models.Public;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Public.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Public, unauthenticated traceability endpoints (swagger <c>security: []</c>).
/// </summary>
[ApiController]
[Route("api/v1/public")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class PublicController : ControllerBase
{
    private readonly ISender _sender;

    public PublicController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Public traceability lookup by batch ID. No authentication required.
    /// </summary>
    [HttpGet("trace/{batchId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetTrace(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(new GetPublicTraceQuery(batchId), cancellationToken);
        return Ok(ApiResponse.Success(ToData(dto)));
    }

    /// <summary>
    /// Batch lineage (split/merge relationships). No authentication required.
    /// </summary>
    [HttpGet("trace/{batchId:guid}/lineage")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetLineage(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(new GetBatchLineageQuery(batchId), cancellationToken);

        var data = new LineageData
        {
            RootBatchId = dto.RootBatchId,
            Lineage = dto.Lineage.Select(n => new LineageNode
            {
                BatchId = n.BatchId,
                BatchCode = n.BatchCode,
                EventTypeCode = n.EventTypeCode,
                Quantity = n.Quantity,
                ParentBatchId = n.ParentBatchId
            }).ToList()
        };

        return Ok(ApiResponse.Success(data));
    }

    private static PublicTraceData ToData(PublicTraceDto dto) => new()
    {
        BatchId = dto.BatchId,
        BatchCode = dto.BatchCode,
        ProductName = dto.ProductName,
        Quantity = dto.Quantity,
        UnitCode = dto.UnitCode,
        CurrentOrganizationName = dto.CurrentOrganizationName,
        Status = dto.Status,
        Timeline = dto.Timeline.Select(t => new TimelineEvent
        {
            EventTypeCode = t.EventTypeCode,
            OrganizationName = t.OrganizationName,
            EventTime = t.EventTime,
            Location = t.Location
        }).ToList(),
        Inspections = dto.Inspections.Select(i => new PublicInspectionSummary
        {
            Result = i.Result,
            InspectorName = i.InspectorName,
            CreatedAt = i.CreatedAt
        }).ToList(),
        Certificates = dto.Certificates.Select(c => new PublicCertificateSummary
        {
            CertificateType = c.CertificateType,
            FileUrl = c.FileUrl,
            IssuedDate = c.IssuedDate
        }).ToList(),
        RecallStatus = dto.RecallStatus
    };
}
