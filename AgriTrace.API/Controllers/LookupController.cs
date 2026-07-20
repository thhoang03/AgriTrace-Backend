using AgriTrace.API.Models;
using AgriTrace.API.Models.Lookup;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Inspections.Queries;
using AgriTrace.Application.Features.Lookup.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Reference/lookup data endpoints. Each action exposes its own root-level route.
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
public sealed class LookupController : ControllerBase
{
    private readonly ISender _sender;

    public LookupController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Danh sách Role
    /// </summary>
    [HttpGet("api/v1/roles")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetRoles(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetRolesQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    /// <summary>
    /// Danh sách loại tổ chức
    /// </summary>
    [HttpGet("api/v1/organization-types")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetOrganizationTypes(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetOrganizationTypesQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    /// <summary>
    /// Danh sách loại Event
    /// </summary>
    [HttpGet("api/v1/event-types")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetEventTypes(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetEventTypesQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    /// <summary>
    /// Danh sách đơn vị tính
    /// </summary>
    [HttpGet("api/v1/units")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetUnits(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetUnitsQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    /// <summary>
    /// Danh sách kết quả kiểm định
    /// </summary>
    [HttpGet("api/v1/inspection-results")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetInspectionResults(CancellationToken cancellationToken)
    {
        var results = await _sender.Send(new GetInspectionResultsQuery(), cancellationToken);

        var items = results
            .Select(r => new LookupItem
            {
                Id = r.Value.ToString(),
                Code = r.Code,
                Name = r.Name
            })
            .ToList();

        return Ok(ApiResponse.Success(items));
    }

    /// <summary>
    /// Danh sách loại chứng nhận
    /// </summary>
    [HttpGet("api/v1/certificate-types")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetCertificateTypes(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetCertificateTypesQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    /// <summary>
    /// Danh sách mức độ Recall
    /// </summary>
    [HttpGet("api/v1/recall-severities")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetRecallSeverities(CancellationToken cancellationToken)
    {
        var items = await _sender.Send(new GetRecallSeveritiesQuery(), cancellationToken);
        return Ok(ApiResponse.Success(ToItems(items)));
    }

    private static List<LookupItem> ToItems(IReadOnlyList<LookupItemDto> items) =>
        items.Select(i => new LookupItem
        {
            Id = i.Id,
            Code = i.Code,
            Name = i.Name
        }).ToList();
}
