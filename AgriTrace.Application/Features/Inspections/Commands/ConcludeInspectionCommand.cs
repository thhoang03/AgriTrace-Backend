using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using MediatR;
using System.Text.Json;

// Need to add using for CreateEventCommand namespace
using AgriTrace.Application.Features.Events.Commands;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record ConcludeInspectionCommand(
    Guid Id,
    string OverallResult,
    string? Notes)
    : IRequest;

public sealed class ConcludeInspectionCommandHandler
    : IRequestHandler<ConcludeInspectionCommand>
{
    private readonly IQualityInspectionService _service;
    private readonly IMediator _mediator;
    private readonly IEventTypeService _eventTypeService;
    private readonly IBatchReadService _batchReadService;
    private readonly ICurrentUserService _currentUser;

    public ConcludeInspectionCommandHandler(
        IQualityInspectionService service,
        IMediator mediator,
        IEventTypeService eventTypeService,
        IBatchReadService batchReadService,
        ICurrentUserService currentUser)
    {
        _service = service;
        _mediator = mediator;
        _eventTypeService = eventTypeService;
        _batchReadService = batchReadService;
        _currentUser = currentUser;
    }

    public async Task Handle(
        ConcludeInspectionCommand command,
        CancellationToken cancellationToken)
    {
        var inspection = await _service.GetByIdAsync(command.Id, cancellationToken);

        if (inspection is null)
        {
            throw new KeyNotFoundException(
                $"Inspection with Id '{command.Id}' was not found.");
        }

        inspection.Conclude(command.OverallResult, command.Notes);

        await _service.UpdateAsync(inspection, cancellationToken);

        await TryCreateInspectionEventAsync(inspection, cancellationToken);
    }

    private async Task TryCreateInspectionEventAsync(
        QualityInspection inspection,
        CancellationToken cancellationToken)
    {
        try
        {
            var eventType = await _eventTypeService.GetByCodeAsync(
                "INSPECTION", cancellationToken);

            if (eventType is null) return;

            var eventData = JsonSerializer.Serialize(new
            {
                inspectionId = inspection.Id,
                overallResult = inspection.OverallResult,
                status = inspection.Status.ToString(),
                action = "concluded"
            });

            await _mediator.Send(new CreateEventCommand(
                inspection.BatchId,
                eventType.Id,
                eventData,
                Location: null,
                _currentUser.UserId,
                inspection.Id), cancellationToken);
        }
        catch
        {
            // Don't fail inspection conclusion if event creation fails
        }
    }
}

public sealed class ConcludeInspectionCommandValidator
    : AbstractValidator<ConcludeInspectionCommand>
{
    public ConcludeInspectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Inspection Id is required.");

        RuleFor(x => x.OverallResult)
            .NotEmpty()
            .WithMessage("OverallResult is required.")
            .Must(r => r == "PASS" || r == "FAIL")
            .WithMessage("OverallResult must be 'PASS' or 'FAIL'.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 2000 characters.");
    }
}
