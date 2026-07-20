using AgriTrace.API.Models;
using AgriTrace.API.Models.Analytics;
using AgriTrace.Application.Features.Analytics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Dashboard and reporting analytics.
/// </summary>
[ApiController]
[Route("api/v1/analytics")]
[Authorize(Roles = "Admin,Manager")]
[Produces("application/json")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly ISender _sender;

    public AnalyticsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Dashboard tổng quan
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetOverview(CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(new GetOverviewQuery(), cancellationToken);

        var data = new OverviewData
        {
            TotalBatches = dto.TotalBatches,
            TotalOrganizations = dto.TotalOrganizations,
            TotalEvents = dto.TotalEvents,
            TotalRecalls = dto.TotalRecalls,
            ActiveBatches = dto.ActiveBatches,
            RecalledBatches = dto.RecalledBatches
        };

        return Ok(ApiResponse.Success(data));
    }

    /// <summary>
    /// Thống kê phân bố Batch theo trạng thái
    /// </summary>
    [HttpGet("batch-distribution")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetBatchDistribution(
        Guid? organizationId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(
            new GetBatchDistributionQuery(organizationId, fromDate, toDate),
            cancellationToken);

        var data = new BatchDistributionData
        {
            Items = dto.Items.Select(i => new BatchStatusDistributionItem
            {
                Status = i.Status,
                StatusName = i.StatusName,
                Count = i.Count
            }).ToList(),
            TotalCount = dto.TotalCount
        };

        return Ok(ApiResponse.Success(data));
    }

    /// <summary>
    /// Thống kê thời gian xử lý
    /// </summary>
    [HttpGet("processing-time")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProcessingTime(
        Guid? organizationId,
        Guid? eventTypeId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(
            new GetProcessingTimeQuery(organizationId, eventTypeId, fromDate, toDate),
            cancellationToken);

        var data = new ProcessingTimeData
        {
            AverageProcessingHours = dto.AverageProcessingHours,
            ByEventType = dto.ByEventType.Select(e => new ProcessingTimeByEventType
            {
                EventTypeCode = e.EventTypeCode,
                AverageHours = e.AverageHours
            }).ToList()
        };

        return Ok(ApiResponse.Success(data));
    }

    /// <summary>
    /// Truy vết ngược từ một Batch
    /// </summary>
    [HttpGet("traceback/{batchId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetTraceback(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dto = await _sender.Send(new GetTracebackQuery(batchId), cancellationToken);

        var data = new TracebackData
        {
            BatchId = dto.BatchId,
            BatchCode = dto.BatchCode,
            AffectedBatches = dto.AffectedBatches.Select(a => new AffectedBatch
            {
                BatchId = a.BatchId,
                BatchCode = a.BatchCode,
                Relationship = a.Relationship
            }).ToList(),
            RelatedOrganizations = dto.RelatedOrganizations.Select(o => new RelatedOrganization
            {
                OrganizationId = o.OrganizationId,
                Name = o.Name,
                Type = o.Type
            }).ToList()
        };

        return Ok(ApiResponse.Success(data));
    }
}
