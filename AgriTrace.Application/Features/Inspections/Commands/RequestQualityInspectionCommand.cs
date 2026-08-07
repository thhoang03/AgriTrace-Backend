using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record RequestQualityInspectionCommand(
    Guid BatchId,
    Guid TargetOrganizationId,
    string? Notes)
    : IRequest<QualityInspectionDto>;

public sealed class RequestQualityInspectionCommandHandler
    : IRequestHandler<RequestQualityInspectionCommand, QualityInspectionDto>
{
    private readonly IQualityInspectionService _service;
    private readonly IBatchReadService _batchReadService;

    public RequestQualityInspectionCommandHandler(
        IQualityInspectionService service,
        IBatchReadService batchReadService)
    {
        _service = service;
        _batchReadService = batchReadService;
    }

    public async Task<QualityInspectionDto> Handle(
        RequestQualityInspectionCommand command,
        CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(command.BatchId, cancellationToken);
        if (batch == null)
            throw new ArgumentException("Batch not found.");

        var inspectionNotes = string.IsNullOrWhiteSpace(command.Notes)
            ? $"Yêu cầu kiểm định chất lượng"
            : command.Notes;

        var inspection = new QualityInspection(
            command.BatchId,
            command.TargetOrganizationId,
            null,
            InspectionType.RawMaterial, // Default or should it be param?
            DateTime.UtcNow,
            inspectionNotes
        );

        var created = await _service.CreateAsync(inspection, cancellationToken);

        return new QualityInspectionDto
        {
            Id = created.Id,
            BatchId = created.BatchId,
            OrganizationId = created.OrganizationId,
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
}

public sealed class RequestQualityInspectionCommandValidator
    : AbstractValidator<RequestQualityInspectionCommand>
{
    public RequestQualityInspectionCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("BatchId is required.");

        RuleFor(x => x.TargetOrganizationId)
            .NotEmpty()
            .WithMessage("TargetOrganizationId is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 2000 characters.");
    }
}
