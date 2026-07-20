using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Application.Features.Organizations.Queries;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý tổ chức (Organization).
/// </summary>
[ApiController]
[Route("api/v1/organizations")]
[Authorize]
public sealed class OrganizationsController : ControllerBase
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách tổ chức
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        Guid? organizationTypeId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (organizationTypeId.HasValue)
        {
            var byType = await _sender.Send(
                new GetOrganizationsByTypeQuery(organizationTypeId.Value),
                cancellationToken);

            return Ok(ApiResponse.Success(byType));
        }

        var result = await _sender.Send(
            new GetOrganizationsPagedQuery(page, pageSize),
            cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Xem chi tiết tổ chức
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOrganizationByIdQuery(id),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                result.ToResponse()));
    }

    /// <summary>
    /// Tạo tổ chức mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Create(
        OrganizationRequest request,
        CancellationToken cancellationToken)
    {        var organization = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        return Created(
            $"/api/v1/organizations/{organization.Id}",
            ApiResponse.Success(
                organization));
    }

    /// <summary>
    /// Cập nhật tổ chức
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Update(
        Guid id,
        OrganizationRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            request.ToCommand(id),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Organization updated successfully."));
    }

    /// <summary>
    /// Thay đổi trạng thái tổ chức
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(
        Guid id,
        [FromBody] StatusRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateOrganizationStatusCommand(id, request.Status),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Organization status updated successfully."));
    }

    /// <summary>
    /// Lấy người dùng thuộc tổ chức
    /// </summary>
    [HttpGet("{id:guid}/users")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetUsers(
        Guid id,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetOrganizationUsersQuery(id, page, pageSize),
            cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Lấy sản phẩm của tổ chức
    /// </summary>
    [HttpGet("{id:guid}/products")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProducts(
        Guid id,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetOrganizationProductsQuery(id, page, pageSize),
            cancellationToken);

        var paged = new ProductPagedResponse(
            result.Items.Select(x => x.ToListItemResponse()),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    // Not in swagger.yaml — internal-only endpoint, suppressed from OpenAPI docs (Phase 12 decision: keep suppressed).
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteOrganizationCommand(id),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Delete organization successfully."));
    }
}