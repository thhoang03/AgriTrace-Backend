using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Categories.Queries;

public record GetCategoriesPagedQuery(
    string? Search,
    int Page,
    int PageSize) : IRequest<PagedResult<CategoryDto>>;

public class GetCategoriesPagedQueryHandler : IRequestHandler<GetCategoriesPagedQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryService _categoryService;

    public GetCategoriesPagedQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesPagedQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _categoryService.GetPagedAsync(
            request.Search,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtoItems = pagedResult.Items.Adapt<List<CategoryDto>>();

        return new PagedResult<CategoryDto>(
            dtoItems,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize);
    }
}
