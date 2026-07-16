using MediatR;
using Microsoft.AspNetCore.Mvc;

using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Batches.Commands;
using AgriTrace.Application.Features.Batches.Queries;


namespace AgriTrace.API.Controllers;


[ApiController]
[Route("api/v1/batches")]
public sealed class BatchesController : ControllerBase
{

    private readonly ISender _sender;


    public BatchesController(
        ISender sender)
    {
        _sender = sender;
    }



    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {

        var result = await _sender.Send(
            new GetBatchesQuery(
                productId,
                organizationId,
                search,
                page,
                pageSize),
            cancellationToken);

        return Ok(result);

    }



    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {

        var result = await _sender.Send(
            new GetBatchByIdQuery(id),
            cancellationToken);

        return Ok(result.ToResponse());

    }



    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] BatchRequest request,
        CancellationToken cancellationToken)
    {

        var batch = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = batch.Id },
            batch.ToResponse());

    }



    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] BatchRequest request,
        CancellationToken cancellationToken)
    {

        var batch = await _sender.Send(
            request.ToCommand(id),
            cancellationToken);

        return Ok(batch.ToResponse());

    }



    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {

        await _sender.Send(
            new DeleteBatchCommand(id),
            cancellationToken);

        return Ok(ApiResponse.Success("Batch deleted successfully."));

    }

}