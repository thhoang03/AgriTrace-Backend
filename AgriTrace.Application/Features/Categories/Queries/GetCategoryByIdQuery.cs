using System;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category {request.Id} not found.");
        }

        return category.Adapt<CategoryDto>();
    }
}
