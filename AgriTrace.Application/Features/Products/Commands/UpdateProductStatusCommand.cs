using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Commands;

public sealed record UpdateProductStatusCommand(
    Guid ProductId,
    ProductStatus Status) : IRequest<ProductDto>;

public sealed class UpdateProductStatusCommandHandler
    : IRequestHandler<UpdateProductStatusCommand, ProductDto>
{
    private readonly IProductWriteService _productWriteService;

    public UpdateProductStatusCommandHandler(IProductWriteService productWriteService)
    {
        _productWriteService = productWriteService;
    }

    public async Task<ProductDto> Handle(UpdateProductStatusCommand command, CancellationToken cancellationToken)
    {
        var product = await _productWriteService.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        // Toggle the in-memory status. NOTE: Product.Status is not yet persisted (no DB column /
        // migration in this phase); the write path below preserves the existing product fields so the
        // entity's UpdatedAt is bumped. Real status persistence is a Phase 11 follow-up.
        product.ChangeStatus(command.Status);

        await _productWriteService.UpdateAsync(
            command.ProductId,
            new UpdateProduct(product.CategoryId, product.UnitId, product.Name),
            cancellationToken);

        var dto = product.Adapt<ProductDto>();

        return dto;
    }
}
