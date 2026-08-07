using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Commands;

public sealed record UpdateProductStatusCommand(
    Guid ProductId,
    ProductStatus Status) : IRequest<ProductDto>;

public sealed class UpdateProductStatusCommandHandler
    : IRequestHandler<UpdateProductStatusCommand, ProductDto>
{
    private readonly IProductWriteRepository _repository;

    public UpdateProductStatusCommandHandler(IProductWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(UpdateProductStatusCommand command, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        product.ChangeStatus(command.Status);

        await _repository.UpdateAsync(product, cancellationToken);

        var dto = product.Adapt<ProductDto>();

        return dto;
    }
}
