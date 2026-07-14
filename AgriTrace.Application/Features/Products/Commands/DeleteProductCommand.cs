using FluentValidation;
using MediatR;
using AgriTrace.Domain.Interfaces.Inbound;

namespace AgriTrace.Application.Features.Products.Commands;

public sealed record DeleteProductCommand(
    Guid Id) : IRequest;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductWriteService _productWriteService;

    public DeleteProductCommandHandler(
        IProductWriteService productWriteService)
    {
        _productWriteService = productWriteService;
    }

    public async Task Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        await _productWriteService.DeleteAsync(
            command.Id,
            cancellationToken);
    }
}

public sealed class DeleteProductCommandValidator
    : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}