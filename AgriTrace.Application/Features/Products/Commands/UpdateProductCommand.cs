using FluentValidation;
using MediatR;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;

namespace AgriTrace.Application.Features.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    Guid? CategoryId,
    Guid? UnitId,
    string Name,
    Guid? OrganizationId = null) : IRequest;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductWriteService _productWriteService;

    public UpdateProductCommandHandler(
        IProductWriteService productWriteService)
    {
        _productWriteService = productWriteService;
    }

    public async Task Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        await _productWriteService.UpdateAsync(
            command.Id,
            command.ToUpdateProduct(),
            cancellationToken);
    }
}

public sealed class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}