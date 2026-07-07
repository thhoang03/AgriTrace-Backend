using System;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Commands
{
    public record CreateProductCommand(
        int CategoryId,
        string Name,
        string? ScientificName,
        string? Description,
        string? ImageUrl,
        int? ShelfLifeDays
    ) : IRequest<ProductDto>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(
                productId: Guid.NewGuid(),
                categoryId: request.CategoryId,
                name: request.Name,
                scientificName: request.ScientificName,
                description: request.Description,
                imageUrl: request.ImageUrl,
                shelfLifeDays: request.ShelfLifeDays
            );

            var addedProduct = await _productRepository.AddAsync(product);
            return addedProduct.Adapt<ProductDto>();
        }
    }
}
