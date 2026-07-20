using System.Linq;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Products.Queries;

public sealed record GetProductsQuery(
    Guid? OrganizationId,
    Guid? CategoryId,
    string? Search,
    int Page,
    int PageSize)
    : IRequest<PaginationResponse<ProductDto>>;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, PaginationResponse<ProductDto>>
{
    private readonly IProductReadService _productReadService;

    public GetProductsQueryHandler(
        IProductReadService productReadService)
    {
        _productReadService = productReadService;
    }

    public async Task<PaginationResponse<ProductDto>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var paged = await _productReadService.SearchAsync(
            query.OrganizationId,
            query.CategoryId,
            query.Search,
            query.Page,
            query.PageSize,
            cancellationToken);

        var items = paged.Items
            .Select(x => x.Adapt<ProductDto>())
            .ToList();

        return new PaginationResponse<ProductDto>(
            items,
            paged.TotalCount,
            query.Page,
            query.PageSize);
    }
}