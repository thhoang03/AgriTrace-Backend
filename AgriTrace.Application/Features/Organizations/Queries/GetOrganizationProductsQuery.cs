using System.Linq;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

/// <summary>
/// Returns the products belonging to an organization. Delegates to the product read service with
/// the organizationId filter. The API layer maps the result into a <c>ProductPagedResponse</c>.
/// </summary>
public sealed record GetOrganizationProductsQuery(
    Guid OrganizationId,
    int Page,
    int PageSize)
    : IRequest<PaginationResponse<ProductDto>>;

public sealed class GetOrganizationProductsQueryHandler
    : IRequestHandler<GetOrganizationProductsQuery, PaginationResponse<ProductDto>>
{
    private readonly IProductReadService _productReadService;

    public GetOrganizationProductsQueryHandler(IProductReadService productReadService)
    {
        _productReadService = productReadService;
    }

    public async Task<PaginationResponse<ProductDto>> Handle(
        GetOrganizationProductsQuery query,
        CancellationToken cancellationToken)
    {
        var paged = await _productReadService.SearchAsync(
            query.OrganizationId,
            categoryId: null,
            search: null,
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
