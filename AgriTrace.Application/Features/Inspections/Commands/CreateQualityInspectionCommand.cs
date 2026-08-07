using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Entities.Notifications;
using FluentValidation;
using MediatR;
using System.Text.Json;

using AgriTrace.Application.Features.Events.Commands;

namespace AgriTrace.Application.Features.Inspections.Commands;

public sealed record CreateQualityInspectionCommand(
    Guid BatchId,
    Guid InspectorId,
    Guid? OrganizationId,
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
    private readonly INotificationService? _notificationService;
    private readonly IUserService? _userService;

    public CreateQualityInspectionCommandHandler(
        IQualityInspectionService service,
        IMediator mediator,
        IEventTypeService eventTypeService,
        IBatchReadService batchReadService,
        ICurrentUserService currentUser,
        INotificationService? notificationService = null,
        IUserService? userService = null)
    {
        _service = service;
        _mediator = mediator;
        _eventTypeService = eventTypeService;
        _batchReadService = batchReadService;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _userService = userService;
    }

    public async Task<QualityInspectionDto> Handle(
        CreateQualityInspectionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.OrganizationId.HasValue && command.OrganizationId.Value != Guid.Empty)
        {
            var existingInspections = await _service.GetByBatchAsync(command.BatchId, cancellationToken);
            var pendingOtherOrg = existingInspections.FirstOrDefault(i =>
                i.OrganizationId.HasValue &&
                i.OrganizationId.Value != command.OrganizationId.Value &&
                i.Status == Domain.Entities.QualityInspections.InspectionStatus.Pending);

            if (pendingOtherOrg != null)
            {
                throw new ArgumentException("Lô hàng này đã được yêu cầu/chỉ định kiểm tra cho một tổ chức kiểm định khác.");
            }
        }

        var inspection = new QualityInspection(
            command.BatchId,
            command.OrganizationId, // use the provided org Id
            command.InspectorId,
            (InspectionType)command.InspectionType,
            command.InspectionDate,
            command.Notes);

        var created = await _service.CreateAsync(inspection, cancellationToken);

        await TryCreateInspectionEventAsync(command, created, cancellationToken);

        if (_notificationService != null && _userService != null)
        {
            var batch = await _batchReadService.GetByIdAsync(command.BatchId, cancellationToken);
            if (batch != null && batch.CurrentOrganizationId != Guid.Empty)
            {
                var orgUsers = await _userService.GetByOrganizationAsync(batch.CurrentOrganizationId, cancellationToken);
                foreach (var u in orgUsers)
                {
                    var titleJson = JsonSerializer.Serialize(new { en = "🔬 NEW INSPECTION RESULT", vi = "🔬 KẾT QUẢ KIỂM ĐỊNH MỚI" });
                    
                    var resultStrVi = created.OverallResult == "PASS" ? "ĐẠT" :
                                      created.OverallResult == "FAIL" ? "KHÔNG ĐẠT" : "CẢNH BÁO";
                    var resultStrEn = created.OverallResult == "PASS" ? "PASS" :
                                      created.OverallResult == "FAIL" ? "FAIL" : "WARNING";

                    var msgJson = JsonSerializer.Serialize(new { 
                        en = $"Batch {batch.BatchCode} has a new inspection result. Overall assessment: {resultStrEn}.",
                        vi = $"Lô hàng {batch.BatchCode} vừa có kết quả kiểm định mới. Kết quả đánh giá chung: {resultStrVi}."
                    });

                    var notif = new Notification(
                        u.Id,
                        titleJson,
                        msgJson);
                    await _notificationService.CreateAsync(notif, cancellationToken);
                }
            }
        }

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
                description = $"Khởi tạo phiếu kiểm định chất lượng",
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
