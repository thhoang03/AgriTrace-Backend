using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Organizations.Commands;
using AgriTrace.Application.Features.Organizations.Queries;

namespace AgriTrace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
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

    [HttpGet("{id:guid}")]
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

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Create(
        OrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var organization = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        return Created(
            $"/api/organizations/{organization.Id}",
            ApiResponse.Success(
                organization,
                HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
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

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateOrganizationStatusCommand(id, isActive),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Organization status updated successfully."));
    }

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