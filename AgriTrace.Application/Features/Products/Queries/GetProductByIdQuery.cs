using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<ProductDto?>;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductReadService _productReadService;

    public GetProductByIdQueryHandler(
        IProductReadService productReadService)
    {
        _productReadService = productReadService;
    }

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await _productReadService.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (product is null)
        {
            return null;
        }

        return product.Adapt<ProductDto>();
    }
}