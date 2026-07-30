using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record AddLabTestCommand(
    Guid InspectionId,
    string TestName,
    string? MeasuredValue,
    string? Unit,
    string? MinStandardValue,
    string? MaxStandardValue,
    bool IsPassed,
    string? Remark)
    : IRequest<InspectionLabTestDto>;

public sealed class AddLabTestCommandHandler
    : IRequestHandler<AddLabTestCommand, InspectionLabTestDto>
{
    private readonly IQualityInspectionService _service;

    public AddLabTestCommandHandler(
        IQualityInspectionService service)
    {
        _service = service;
    }

    public async Task<InspectionLabTestDto> Handle(
        AddLabTestCommand command,
        CancellationToken cancellationToken)
    {
        var inspection = await _service.GetByIdWithLabTestsAsync(
            command.InspectionId, cancellationToken);

        if (inspection is null)
        {
            throw new KeyNotFoundException(
                $"Inspection with Id '{command.InspectionId}' was not found.");
        }

        if (inspection.Status != Domain.Entities.QualityInspections.InspectionStatus.Pending)
        {
            throw new InvalidOperationException(
                "Cannot add lab tests to an inspection that is not in Pending status.");
        }

        var labTest = new InspectionLabTest(
            command.InspectionId,
            command.TestName,
            command.MeasuredValue,
            command.Unit,
            command.MinStandardValue,
            command.MaxStandardValue,
            command.IsPassed,
            command.Remark);

        await _service.AddLabTestAsync(labTest, cancellationToken);

        return new InspectionLabTestDto
        {
            Id = labTest.Id,
            InspectionId = labTest.InspectionId,
            TestName = labTest.TestName,
            MeasuredValue = labTest.MeasuredValue,
            Unit = labTest.Unit,
            MinStandardValue = labTest.MinStandardValue,
            MaxStandardValue = labTest.MaxStandardValue,
            IsPassed = labTest.IsPassed,
            Remark = labTest.Remark,
            CreatedAt = labTest.CreatedAt
        };
    }
}

public sealed class AddLabTestCommandValidator
    : AbstractValidator<AddLabTestCommand>
{
    public AddLabTestCommandValidator()
    {
        RuleFor(x => x.InspectionId)
            .NotEmpty()
            .WithMessage("InspectionId is required.");

        RuleFor(x => x.TestName)
            .NotEmpty()
            .WithMessage("Test name is required.")
            .MaximumLength(200)
            .WithMessage("Test name must not exceed 200 characters.");

        RuleFor(x => x.MeasuredValue)
            .MaximumLength(200)
            .When(x => x.MeasuredValue != null);

        RuleFor(x => x.Unit)
            .MaximumLength(50)
            .When(x => x.Unit != null);

        RuleFor(x => x.MinStandardValue)
            .MaximumLength(200)
            .When(x => x.MinStandardValue != null);

        RuleFor(x => x.MaxStandardValue)
            .MaximumLength(200)
            .When(x => x.MaxStandardValue != null);

        RuleFor(x => x.Remark)
            .MaximumLength(500)
            .When(x => x.Remark != null);
    }
}
