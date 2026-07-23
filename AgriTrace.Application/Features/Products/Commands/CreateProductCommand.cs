using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    Guid OrganizationId,
    Guid? CategoryId,
    Guid? UnitId,
    string Name)
    : IRequest<Product>;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductWriteService _productWriteService;

    public CreateProductCommandHandler(
        IProductWriteService productWriteService)
    {
        _productWriteService = productWriteService;
    }

    public Task<Product> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        return _productWriteService.CreateAsync(
            command.ToNewProduct(),
            cancellationToken);
    }
}

public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.UnitId)
            .NotEqual(Guid.Empty)
            .When(x => x.UnitId.HasValue);
    }
}
