using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using MediatR;
using System.Text.Json;

using AgriTrace.Application.Features.Events.Commands;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record CreateQualityInspectionCommand(
    Guid BatchId,
    Guid InspectorId,
    int InspectionType,
    DateTime InspectionDate,
    string? Notes)
    : IRequest<QualityInspectionDto>;

public sealed class CreateQualityInspectionCommandHandler
    : IRequestHandler<CreateQualityInspectionCommand, QualityInspectionDto>
{
    private readonly IQualityInspectionService _service;
    private readonly IMediator _mediator;
    private readonly IEventTypeService _eventTypeService;
    private readonly IBatchReadService _batchReadService;
    private readonly ICurrentUserService _currentUser;

    public CreateQualityInspectionCommandHandler(
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

    public async Task<QualityInspectionDto> Handle(
        CreateQualityInspectionCommand command,
        CancellationToken cancellationToken)
    {
        var inspection = new QualityInspection(
            command.BatchId,
            command.InspectorId,
            (InspectionType)command.InspectionType,
            command.InspectionDate,
            command.Notes);

        var created = await _service.CreateAsync(inspection, cancellationToken);

        await TryCreateInspectionEventAsync(command, created, cancellationToken);

        return new QualityInspectionDto
        {
            Id = created.Id,
            BatchId = created.BatchId,
            InspectorId = created.InspectorId,
            InspectionType = (int)created.InspectionType,
            Status = (int)created.Status,
            OverallResult = created.OverallResult,
            InspectionDate = created.InspectionDate,
            Notes = created.Notes,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }

    private async Task TryCreateInspectionEventAsync(
        CreateQualityInspectionCommand command,
        QualityInspection created,
        CancellationToken cancellationToken)
    {
        try
        {
            var eventType = await _eventTypeService.GetByCodeAsync(
                "INSPECTION", cancellationToken);

            if (eventType is null) return;

            var eventData = JsonSerializer.Serialize(new
            {
                inspectionId = created.Id,
                inspectionType = command.InspectionType,
                status = (int)created.Status,
                action = "created"
            });

            await _mediator.Send(new CreateEventCommand(
                created.BatchId,
                eventType.Id,
                eventData,
                Location: null,
                _currentUser.UserId,
                created.Id), cancellationToken);
        }
        catch
        {
            // Don't fail inspection creation if event creation fails
        }
    }
}

public sealed class CreateQualityInspectionCommandValidator
    : AbstractValidator<CreateQualityInspectionCommand>
{
    public CreateQualityInspectionCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("BatchId is required.");

        RuleFor(x => x.InspectionType)
            .Must(t => Enum.IsDefined(typeof(InspectionType), t))
            .WithMessage("InspectionType must be a valid value (1-7).");

        RuleFor(x => x.InspectionDate)
            .NotEmpty()
            .WithMessage("InspectionDate is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 2000 characters.");
    }
}
