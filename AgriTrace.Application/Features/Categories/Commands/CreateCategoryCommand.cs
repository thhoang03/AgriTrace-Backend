using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Categories.Commands;

public record CreateCategoryCommand(
    [Required] [StringLength(100, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Description) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryService.GetByNameAsync(request.Name, cancellationToken) != null)
        {
            throw new ConflictException("Category name already exists.");
        }

        var category = new Category(request.Name, request.Description);

        try
        {
            var created = await _categoryService.CreateAsync(category, cancellationToken);
            return created.Adapt<CategoryDto>();
        }
        catch (DuplicateEntityException)
        {
            throw new ConflictException("Category name already exists.");
        }
    }
}
