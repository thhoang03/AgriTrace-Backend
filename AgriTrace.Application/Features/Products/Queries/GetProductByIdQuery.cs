using System;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto?>;

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return null;
            }

            return product.Adapt<ProductDto>();
        }
    }
}
