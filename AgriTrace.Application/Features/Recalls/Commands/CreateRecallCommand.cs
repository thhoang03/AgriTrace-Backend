using AgriTrace.Application.Common.Exceptions;
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
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Recalls.Commands;

/// <summary>
/// Result of creating a recall.
/// </summary>
public class RecallCreatedResult
{
    public Guid RecallId { get; set; }
}

public record CreateRecallCommand(
    Guid BatchId,
    string Reason,
    int Severity,
    Guid CreatedByUserId) : IRequest<RecallCreatedResult>;

public class CreateRecallCommandHandler : IRequestHandler<CreateRecallCommand, RecallCreatedResult>
{
    private readonly IRecallService _recallService;
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IBatchSplitService? _batchSplitService;
    private readonly IBatchMergeService? _batchMergeService;
    private readonly IEventTypeRepository? _eventTypeRepository;
    private readonly IEventService? _eventService;
    private readonly INotificationService? _notificationService;

    public CreateRecallCommandHandler(
        IRecallService recallService,
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IUserService userService,
        ICurrentUserService currentUser,
        IBatchSplitService? batchSplitService = null,
        IBatchMergeService? batchMergeService = null,
        IEventTypeRepository? eventTypeRepository = null,
        IEventService? eventService = null,
        INotificationService? notificationService = null)
    {
        _recallService = recallService;
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _userService = userService;
        _currentUser = currentUser;
        _batchSplitService = batchSplitService;
        _batchMergeService = batchMergeService;
        _eventTypeRepository = eventTypeRepository;
        _eventService = eventService;
        _notificationService = notificationService;
    }

    public async Task<RecallCreatedResult> Handle(CreateRecallCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Authentication required to create recalls.");
        }

        if (request.Severity is < 1 or > 3)
        {
            throw new ArgumentException("Severity must be between 1 and 3.");
        }

        var primaryBatch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        if (_currentUser.Role != "Admin" && _currentUser.Role != "ADMIN" && _currentUser.OrganizationType != "SYSTEM")
        {
            if (_currentUser.OrganizationType != "INSPECTION")
            {
                throw new RbacForbiddenException("RBAC_FORBIDDEN", "Only system administrator or inspection organization can create recall events.");
            }
        }

        if (!primaryBatch.CanBeRecalled()) {
            throw new ConflictException($"Batch {request.BatchId} is already under an active recall.");
        }

        var createdBy = request.CreatedByUserId;
        if (createdBy == Guid.Empty)
        {
            createdBy = _currentUser.UserId;
            if (createdBy == Guid.Empty)
            {
                var users = await _userService.GetAllAsync(cancellationToken);
                createdBy = users.FirstOrDefault()?.Id
                    ?? throw new ArgumentException("Cannot create recall without a valid user context.");
            }
        }

        var recall = new Recall(
            request.BatchId,
            createdBy,
            request.Reason,
            (RecallSeverity)request.Severity);

        var created = await _recallService.CreateAsync(recall, cancellationToken);

        // --- CASCADING GRAPH TRAVERSAL (Split & Merge Descendants) ---
        var affectedBatchIds = new HashSet<Guid> { request.BatchId };
        var queue = new Queue<Guid>();
        queue.Enqueue(request.BatchId);

        IReadOnlyList<BatchMerge>? allMerges = null;
        if (_batchMergeService != null)
        {
            allMerges = await _batchMergeService.GetAllAsync(cancellationToken);
        }

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            // 1. Trace Split children
            if (_batchSplitService != null)
            {
                var splits = await _batchSplitService.GetBySourceBatchAsync(currentId, cancellationToken);
                foreach (var split in splits)
                {
                    foreach (var detail in split.Details)
                    {
                        if (affectedBatchIds.Add(detail.TargetBatchId))
                        {
                            queue.Enqueue(detail.TargetBatchId);
                        }
                    }
                }
            }

            // 2. Trace Merge target batches
            if (allMerges != null)
            {
                foreach (var merge in allMerges.Where(m => m.Sources.Any(s => s.SourceBatchId == currentId)))
                {
                    if (affectedBatchIds.Add(merge.NewBatchId))
                    {
                        queue.Enqueue(merge.NewBatchId);
                    }
                }
            }
        }

        // --- APPLY RECALL STATUS, SEND NOTIFICATIONS & LOG LEDGER EVENTS FOR ALL AFFECTED BATCHES ---
        EventType? recallEventType = null;
        if (_eventTypeRepository != null)
        {
            recallEventType = await _eventTypeRepository.GetByCodeAsync("RECALL", cancellationToken)
                ?? await _eventTypeRepository.GetByCodeAsync("RECALL_INITIATED", cancellationToken);
        }

        var notifiedOrgIds = new HashSet<Guid>();

        foreach (var batchId in affectedBatchIds)
        {
            var targetBatch = await _batchReadService.GetByIdAsync(batchId, cancellationToken);
            if (targetBatch is null) continue;

            if (targetBatch.CanBeRecalled())
            {
                targetBatch.Recall();
                await _batchWriteService.UpdateAsync(targetBatch, cancellationToken);
            }

            // Record SupplyChainEvent to Blockchain Ledger
            if (recallEventType != null && _eventService != null)
            {
                var eventData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    description = $"THU HỒI KHẨN CẤP lô hàng {targetBatch.BatchCode}. Lý do: {request.Reason}",
                    recallId = created.Id,
                    severity = request.Severity,
                    primaryBatchId = request.BatchId,
                    primaryBatchCode = primaryBatch.BatchCode
                });

                var recallEvent = new SupplyChainEvent(
                    targetBatch.Id,
                    recallEventType.Id,
                    targetBatch.CurrentOrganizationId,
                    createdBy,
                    eventData: eventData,
                    location: "Safety & Recall Center",
                    inspectionId: null,
                    previousHash: null,
                    currentHash: null);

                await _eventService.CreateEventAsync(recallEvent, cancellationToken);
            }

            // Send Stakeholder Notifications
            if (_notificationService != null && notifiedOrgIds.Add(targetBatch.CurrentOrganizationId))
            {
                var usersInOrg = await _userService.GetByOrganizationAsync(targetBatch.CurrentOrganizationId, cancellationToken);
                foreach (var u in usersInOrg)
                {
                    var titleJson = System.Text.Json.JsonSerializer.Serialize(new { en = "🚨 BATCH RECALL ALERT", vi = "🚨 CẢNH BÁO THU HỒI LÔ HÀNG" });
                    var msgJson = System.Text.Json.JsonSerializer.Serialize(new { 
                        en = $"ALERT: Batch {targetBatch.BatchCode} belonging to your organization is being RECALLED due to safety inspection failure. Reason: {request.Reason}. Please halt distribution and isolate the product!",
                        vi = $"CẢNH BÁO: Lô hàng {targetBatch.BatchCode} của đơn vị bạn bị THU HỒI do ảnh hưởng từ đợt kiểm tra an toàn. Lý do: {request.Reason}. Vui lòng dừng phân phối và cách ly sản phẩm!"
                    });

                    var notif = new Notification(
                        u.Id,
                        titleJson,
                        msgJson);
                    await _notificationService.CreateAsync(notif, cancellationToken);
                }
            }
        }
        
        return new RecallCreatedResult
        {
            RecallId = created.Id
        };
    }
}

