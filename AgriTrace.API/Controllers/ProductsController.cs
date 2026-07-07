using System;
using System.Threading.Tasks;
using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Application.Features.Products.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] ProductRequest request)
        {
            var command = request.Adapt<CreateProductCommand>();
            var resultDto = await _sender.Send(command);
            var response = resultDto.Adapt<ProductResponse>();

            return CreatedAtAction(nameof(GetProductById), new { id = response.ProductId }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetProductById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var resultDto = await _sender.Send(query);

            if (resultDto == null)
            {
                return NotFound();
            }

            var response = resultDto.Adapt<ProductResponse>();
            return Ok(response);
        }
    }
}
