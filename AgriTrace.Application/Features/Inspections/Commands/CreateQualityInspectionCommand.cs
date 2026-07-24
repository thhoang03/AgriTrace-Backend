using AgriTrace.Application.Contracts;
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

public sealed record CreateQualityInspectionCommand(
    Guid BatchId,
    Guid InspectorId,
    string Result,
    string? Notes)
    : IRequest<QualityInspectionDto>;
public sealed class CreateQualityInspectionCommandHandler
    : IRequestHandler<CreateQualityInspectionCommand, QualityInspectionDto>
{
    private readonly IQualityInspectionService _service;

    public CreateQualityInspectionCommandHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<QualityInspectionDto> Handle(
        CreateQualityInspectionCommand command,
        CancellationToken cancellationToken)
    {
        var inspection = new QualityInspection(
            command.BatchId,
            command.InspectorId,
            InspectionStatus.Pending,
            command.Result,
            command.Notes);

        var created = await _service.CreateAsync(inspection, cancellationToken);

        return new QualityInspectionDto
        {
            Id = created.Id,
            BatchId = created.BatchId,
            InspectorId = created.InspectorId,
            Status = (int)created.Status,
            Result = created.Result,
            Notes = created.Notes,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
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

