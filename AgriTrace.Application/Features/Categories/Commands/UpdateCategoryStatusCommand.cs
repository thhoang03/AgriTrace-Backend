using System;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Categories.Commands;

public record UpdateCategoryStatusCommand(
    Guid Id,
    bool IsActive) : IRequest<CategoryDto>;

public class UpdateCategoryStatusCommandHandler : IRequestHandler<UpdateCategoryStatusCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public UpdateCategoryStatusCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category {request.Id} not found.");
        }

        try
        {
            category.ChangeStatus(request.IsActive);
            await _categoryService.UpdateAsync(category, cancellationToken);
        }
        catch
        {
            throw new ConflictException("Failed to update status.");
        }

        return category.Adapt<CategoryDto>();
    }
}
