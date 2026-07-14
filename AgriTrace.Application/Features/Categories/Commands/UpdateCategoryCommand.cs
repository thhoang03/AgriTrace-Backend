using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    [Required] [StringLength(100, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Description) : IRequest<CategoryDto>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public UpdateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _categoryService.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException($"Category {request.Id} not found.");
        }

        var duplicate = await _categoryService.GetByNameAsync(request.Name, cancellationToken);
        if (duplicate != null && duplicate.Id != request.Id)
        {
            throw new ConflictException("Category name already exists.");
        }

        existing.UpdateInformation(request.Name, request.Description);
        await _categoryService.UpdateAsync(existing, cancellationToken);

        return existing.Adapt<CategoryDto>();
    }
}
