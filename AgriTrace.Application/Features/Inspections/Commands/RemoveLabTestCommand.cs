using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record RemoveLabTestCommand(Guid LabTestId) : IRequest;

public sealed class RemoveLabTestCommandHandler
    : IRequestHandler<RemoveLabTestCommand>
{
    private readonly IQualityInspectionService _service;

    public RemoveLabTestCommandHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task Handle(
        RemoveLabTestCommand command,
        CancellationToken cancellationToken)
    {
        var labTest = await _service.GetLabTestByIdAsync(
            command.LabTestId, cancellationToken);

        if (labTest is null)
        {
            throw new KeyNotFoundException(
                $"Lab test with Id '{command.LabTestId}' was not found.");
        }

        await _service.DeleteLabTestAsync(
            command.LabTestId, cancellationToken);
    }
}

public sealed class RemoveLabTestCommandValidator
    : AbstractValidator<RemoveLabTestCommand>
{
    public RemoveLabTestCommandValidator()
    {
        RuleFor(x => x.LabTestId)
            .NotEmpty()
            .WithMessage("LabTestId is required.");
    }
}
