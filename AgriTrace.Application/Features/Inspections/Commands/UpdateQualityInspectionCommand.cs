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

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record UpdateQualityInspectionCommand(
    Guid Id,
    string Result,
    string? Notes)
    : IRequest;

public sealed class UpdateQualityInspectionCommandHandler
    : IRequestHandler<UpdateQualityInspectionCommand>
{
    private readonly IQualityInspectionService _service;

    public UpdateQualityInspectionCommandHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task Handle(
        UpdateQualityInspectionCommand command,
        CancellationToken cancellationToken)
    {
        var inspection = await _service.GetByIdAsync(command.Id, cancellationToken);

        if (inspection is null)
        {
            throw new KeyNotFoundException($"Inspection with Id '{command.Id}' was not found.");
        }

        // Determine status from result string
        var status = command.Result == "PASS"
            ? InspectionStatus.Passed
            : InspectionStatus.Failed;

        inspection.UpdateResult(status, command.Result, command.Notes);

        await _service.UpdateAsync(inspection, cancellationToken);
    }
}

public sealed class UpdateQualityInspectionCommandValidator
    : AbstractValidator<UpdateQualityInspectionCommand>
{
    public UpdateQualityInspectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Inspection Id is required.");

        RuleFor(x => x.Result)
            .NotEmpty()
            .WithMessage("Result is required.")
            .Must(r => r == "PASS" || r == "FAIL")
            .WithMessage("Result must be 'PASS' or 'FAIL'.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 1000 characters.");
    }
}

