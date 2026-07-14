using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Application.Features.Products.Queries;

namespace AgriTrace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAll(
        Guid? organizationId,
        Guid? categoryId,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetProductsQuery(
                organizationId,
                categoryId,
                search,
                page,
                pageSize),
            cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    "Product not found."));
        }

        return Ok(
            ApiResponse.Success(
                result.ToResponse()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Create(
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        return Created(
            $"/api/products/{product.Id}",
            ApiResponse.Success(
                product,
                HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Update(
        Guid id,
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            request.ToCommand(id),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Product updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Delete product successfully."));
    }
}