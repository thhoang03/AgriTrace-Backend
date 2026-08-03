using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.EventRequests.Commands;

public record CreateEventRequestCommand(
    Guid BatchId,
    Guid EventTypeId,
    string? Location,
    string? Description,
    string? EventData) : IRequest<EventRequestDto>;

public class CreateEventRequestCommandHandler : IRequestHandler<CreateEventRequestCommand, EventRequestDto>
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly IBatchReadService _batchReadService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly ISupplyChainEventRepository _eventRepository;

    public CreateEventRequestCommandHandler(
        IEventRequestRepository eventRequestRepository,
        IBatchReadService batchReadService,
        IUserService userService,
        ICurrentUserService currentUser,
        IOrganizationRepository organizationRepository,
        IEventTypeRepository eventTypeRepository,
        ISupplyChainEventRepository eventRepository)
    {
        _eventRequestRepository = eventRequestRepository;
        _batchReadService = batchReadService;
        _userService = userService;
        _currentUser = currentUser;
        _organizationRepository = organizationRepository;
        _eventTypeRepository = eventTypeRepository;
        _eventRepository = eventRepository;
    }

    public async Task<EventRequestDto> Handle(CreateEventRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new ForbiddenException("User is not authenticated");

        var user = await _userService.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {_currentUser.UserId} not found");

        // 1. Verify EventTypeId exists in DB (or fallback to first EventType)
        Guid finalEventTypeId = request.EventTypeId;
        EventType? targetEventType = null;
        if (finalEventTypeId != Guid.Empty)
        {
            targetEventType = await _eventTypeRepository.GetByIdAsync(finalEventTypeId, cancellationToken);
            if (targetEventType == null)
            {
                finalEventTypeId = Guid.Empty;
            }
        }
        if (finalEventTypeId == Guid.Empty)
        {
            var allTypes = await _eventTypeRepository.GetAllAsync(cancellationToken);
            targetEventType = allTypes.FirstOrDefault();
            finalEventTypeId = targetEventType?.Id 
                ?? throw new ConflictException("No EventType exists in the system.");
        }

        // 2. Verify BatchId exists in DB (or fallback to first Batch)
        Guid finalBatchId = request.BatchId;
        if (finalBatchId != Guid.Empty)
        {
            var batch = await _batchReadService.GetByIdAsync(finalBatchId, cancellationToken);
            if (batch == null)
            {
                finalBatchId = Guid.Empty;
            }
        }
        if (finalBatchId == Guid.Empty)
        {
            var allBatches = await _batchReadService.GetAllAsync(cancellationToken);
            if (allBatches.Count > 0)
            {
                finalBatchId = allBatches.First().Id;
            }
            else
            {
                throw new ConflictException("No Batch exists in the system to submit an event request.");
            }
        }

        // 3. Verify OrganizationId exists in DB (or fallback to first Organization)
        Guid organizationId = user.OrganizationId ?? Guid.Empty;
        if (organizationId != Guid.Empty)
        {
            var org = await _organizationRepository.GetByIdAsync(organizationId, cancellationToken);
            if (org == null)
            {
                organizationId = Guid.Empty;
            }
        }
        if (organizationId == Guid.Empty)
        {
            var allOrgs = await _organizationRepository.GetAllAsync(cancellationToken);
            organizationId = allOrgs.FirstOrDefault()?.Id 
                ?? throw new ConflictException("No Organization exists in the system.");
        }

        // 4. Validate Event Sequence Prerequisites
        var targetCode = targetEventType?.Code?.ToUpper() ?? "";
        if (targetCode != "HARVEST" && _eventRepository != null)
        {
            var existingEvents = await _eventRepository.GetByBatchAsync(finalBatchId, cancellationToken);
            var existingCodes = existingEvents
                .Select(e => e.EventType?.Code?.ToUpper() ?? "")
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();

            if (targetCode == "TRANSPORT" && !existingCodes.Any(c => c == "HARVEST" || c == "PACKAGING" || c == "PROCESSING" || c == "RECEIVE" || c == "SPLIT" || c == "MERGE"))
            {
                throw new ConflictException("Lô hàng chưa được thu hoạch hoặc tiếp nhận/đóng gói. Không thể tạo yêu cầu Vận chuyển (TRANSPORT).");
            }
            if (targetCode == "PROCESSING" && !existingCodes.Any(c => c == "RECEIVE" || c == "HARVEST"))
            {
                throw new ConflictException("Lô hàng chưa được tiếp nhận hoặc thu hoạch. Không thể tạo yêu cầu Chế biến (PROCESSING).");
            }
            if (targetCode == "PACKAGING" && !existingCodes.Any(c => c == "PROCESSING" || c == "RECEIVE" || c == "HARVEST"))
            {
                throw new ConflictException("Lô hàng chưa được chế biến hoặc tiếp nhận/thu hoạch. Không thể tạo yêu cầu Đóng gói (PACKAGING).");
            }
            if (targetCode == "DISTRIBUTION" && !existingCodes.Any(c => c == "TRANSPORT" || c == "PACKAGING" || c == "RECEIVE"))
            {
                throw new ConflictException("Lô hàng chưa trải qua vận chuyển hoặc đóng gói. Không thể tạo yêu cầu Phân phối (DISTRIBUTION).");
            }
            if (targetCode == "RETAIL" && !existingCodes.Any(c => c == "DISTRIBUTION" || c == "RECEIVE"))
            {
                throw new ConflictException("Lô hàng chưa được phân phối hoặc tiếp nhận tại điểm bán. Không thể tạo yêu cầu Bán lẻ (RETAIL).");
            }
        }

        var entity = new EventRequest(
            finalBatchId,
            finalEventTypeId,
            organizationId,
            _currentUser.UserId,
            request.EventData,
            request.Location,
            request.Description
        );

        await _eventRequestRepository.AddAsync(entity, cancellationToken);

        // Fetch re-populated entity
        var created = await _eventRequestRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;

        return MapToDto(created);
    }

    public static EventRequestDto MapToDto(EventRequest req)
    {
        return new EventRequestDto
        {
            Id = req.Id,
            BatchId = req.BatchId,
            BatchCode = req.Batch?.BatchCode,
            EventTypeId = req.EventTypeId,
            EventTypeCode = req.EventType?.Code,
            EventTypeName = req.EventType?.Name,
            OrganizationId = req.OrganizationId,
            OrganizationName = req.Organization?.Name,
            RequestedByUserId = req.RequestedByUserId,
            RequestedByUserName = req.RequestedByUser?.FullName,
            EventData = req.EventData,
            Location = req.Location,
            Description = req.Description,
            Status = req.Status,
            RejectionReason = req.RejectionReason,
            CreatedAt = req.CreatedAt,
            ReviewedAt = req.ReviewedAt,
            ReviewedByUserId = req.ReviewedByUserId
        };
    }
}
