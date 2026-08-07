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

namespace AgriTrace.Application.Features.Batches.Commands;

public record SplitDetailInput(
    decimal Quantity,
    Guid UnitId);

/// <summary>
/// Result of a split. Mirrors swagger <c>SplitBatchData</c>.
/// </summary>
public class SplitBatchResult
{
    public Guid ParentBatchId { get; set; }

    public List<Guid> ChildBatchIds { get; set; } = new();
}

public record SplitBatchCommand(
    Guid BatchId,
    List<SplitDetailInput> Splits,
    Guid PerformedByUserId) : IRequest<SplitBatchResult>;

public class SplitBatchCommandHandler : IRequestHandler<SplitBatchCommand, SplitBatchResult>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;
    private readonly IBatchSplitRepository _splitRepository;
    private readonly IEventTypeRepository? _eventTypeRepository;
    private readonly IEventService? _eventService;
    private readonly ICurrentUserService? _currentUser;

    public SplitBatchCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService,
        IBatchSplitRepository splitRepository,
        IEventTypeRepository? eventTypeRepository = null,
        IEventService? eventService = null,
        ICurrentUserService? currentUser = null)
    {
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
        _splitRepository = splitRepository;
        _eventTypeRepository = eventTypeRepository;
        _eventService = eventService;
        _currentUser = currentUser;
    }

    public async Task<SplitBatchResult> Handle(SplitBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.Splits is null || request.Splits.Count < 2)
        {
            throw new ArgumentException("A split requires at least two child batches.");
        }

        var parent = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        if (parent.Status == BatchStatus.Recalled)
        {
            throw new ConflictException($"Lô hàng {parent.BatchCode} đang ở trạng thái bị thu hồi (Recalled), không thể thực hiện tách lô.");
        }

        var split = new BatchSplit(parent.Id);

        var childIds = new List<Guid>();

        var childBatches = new List<Batch>();

        // Create child batches (this reduces the parent's remaining quantity per child).
        foreach (var detail in request.Splits)
        {
            var childCode = Guid.NewGuid().ToString("N")[..8].ToUpper();

            var child = parent.CreateChildBatch(childCode, detail.Quantity, split.Id);

            await _batchWriteService.CreateAsync(child, cancellationToken);

            split.AddDetail(child.Id, detail.Quantity);
            childIds.Add(child.Id);
            childBatches.Add(child);
        }

        // Persist the split audit record and the parent's updated remaining quantity.
        await _splitRepository.AddAsync(split, cancellationToken);
        await _batchWriteService.UpdateAsync(parent, cancellationToken);

        // Record immutable SupplyChainEvents to ledger
        if (_eventTypeRepository != null && _eventService != null)
        {
            var splitEventType = await _eventTypeRepository.GetByCodeAsync("SPLIT", cancellationToken);
            var createdEventType = await _eventTypeRepository.GetByCodeAsync("CREATED", cancellationToken);

            var performedBy = request.PerformedByUserId != Guid.Empty
                ? request.PerformedByUserId
                : (_currentUser?.UserId ?? Guid.Empty);

            if (performedBy == Guid.Empty)
            {
                performedBy = new Guid("70000000-0000-0000-0000-000000000002");
            }

            // 1. SPLIT Event on Parent Batch
            if (splitEventType != null)
            {
                var eventData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    description = $"Tách lô hàng {parent.BatchCode} thành {childIds.Count} lô con.",
                    parentBatchId = parent.Id,
                    parentBatchCode = parent.BatchCode,
                    childBatchIds = childIds
                });

                var splitEvent = new SupplyChainEvent(
                    parent.Id,
                    splitEventType.Id,
                    parent.CurrentOrganizationId,
                    performedBy,
                    eventData: eventData,
                    location: "Processing Facility",
                    inspectionId: null,
                    previousHash: null,
                    currentHash: null);

                await _eventService.CreateEventAsync(splitEvent, cancellationToken);
            }

            // 2. CREATED and SPLIT Events on EACH Child Batch
            foreach (var child in childBatches)
            {
                if (createdEventType != null)
                {
                    var childCreatedEvent = new SupplyChainEvent(
                        child.Id,
                        createdEventType.Id,
                        child.CurrentOrganizationId,
                        performedBy,
                        eventData: $"Lô hàng con {child.BatchCode} được khởi tạo từ việc tách lô {parent.BatchCode}. Sản lượng: {child.RemainingQuantity}",
                        location: "Processing Facility",
                        inspectionId: null,
                        previousHash: null,
                        currentHash: null);

                    await _eventService.CreateEventAsync(childCreatedEvent, cancellationToken);
                }

                if (splitEventType != null)
                {
                    var childSplitEvent = new SupplyChainEvent(
                        child.Id,
                        splitEventType.Id,
                        child.CurrentOrganizationId,
                        performedBy,
                        eventData: $"Tách ra từ lô hàng mẹ {parent.BatchCode}.",
                        location: "Processing Facility",
                        inspectionId: null,
                        previousHash: null,
                        currentHash: null);

                    await _eventService.CreateEventAsync(childSplitEvent, cancellationToken);
                }
            }
        }

        return new SplitBatchResult
        {
            ParentBatchId = parent.Id,
            ChildBatchIds = childIds
        };
    }
}

