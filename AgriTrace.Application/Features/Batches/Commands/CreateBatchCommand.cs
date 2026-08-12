using AgriTrace.Application.Common.Exceptions;
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
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Commands;


public sealed record CreateBatchCommand(
    Guid ProductId,
    Guid UnitId,
    decimal Quantity,
    DateTime ProductionDate,
    DateTime? ExpiryDate,
    string? Location = null)
    : IRequest<BatchDto>;



public sealed class CreateBatchCommandHandler
    : IRequestHandler<CreateBatchCommand, BatchDto>
{

    private readonly IBatchWriteService _batchWriteService;
    private readonly IProductReadService? _productReadService;
    private readonly IEventTypeRepository? _eventTypeRepository;
    private readonly IEventService? _eventService;
    private readonly ICurrentUserService? _currentUser;
    private readonly INotificationService? _notificationService;
    private readonly IUserService? _userService;
    private readonly AgriTrace.Domain.Interfaces.Outbound.IEventRequestRepository? _eventRequestRepository;

    public CreateBatchCommandHandler(
        IBatchWriteService batchWriteService,
        IProductReadService? productReadService = null,
        IEventTypeRepository? eventTypeRepository = null,
        IEventService? eventService = null,
        ICurrentUserService? currentUser = null,
        INotificationService? notificationService = null,
        IUserService? userService = null,
        AgriTrace.Domain.Interfaces.Outbound.IEventRequestRepository? eventRequestRepository = null)
    {
        _batchWriteService = batchWriteService;
        _productReadService = productReadService;
        _eventTypeRepository = eventTypeRepository;
        _eventService = eventService;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _userService = userService;
        _eventRequestRepository = eventRequestRepository;
    }

    public async Task<BatchDto> Handle(
        CreateBatchCommand command,
        CancellationToken cancellationToken)
    {
        if (_currentUser != null && _currentUser.IsAuthenticated && _currentUser.Role != "Admin" && _currentUser.OrganizationType != "FARM" && _currentUser.OrganizationType != "SYSTEM")
        {
            bool hasExtraPermission = false;
            if (_eventRequestRepository != null && _currentUser.OrganizationId.HasValue)
            {
                var eventRequests = await _eventRequestRepository.GetPagedAsync(1, 100, null, AgriTrace.Domain.Enums.EventRequestStatus.Approved, _currentUser.OrganizationId.Value, null, cancellationToken);
                foreach (var req in eventRequests.Items)
                {
                    var code = req.EventType?.Code?.ToUpper();
                    if (code == "CREATED" || code == "CREATE" || code == "HARVEST")
                    {
                        hasExtraPermission = true;
                        break;
                    }
                }
            }

            if (!hasExtraPermission)
            {
                throw new ForbiddenException("Chỉ đơn vị Trang trại (FARM), Admin hệ thống hoặc các tài khoản được phê duyệt mở rộng mới được phép khởi tạo Lô hàng mới.");
            }
        }

        // Server-side batch code generation
        var batchCode = Guid.NewGuid().ToString("N")[..8].ToUpper();

        var batch = new Batch(
            command.ProductId,
            batchCode,
            command.Quantity,
            command.UnitId,
            command.ProductionDate,
            command.ExpiryDate);

        if (_productReadService != null)
        {
            var product = await _productReadService.GetByIdAsync(command.ProductId, cancellationToken);
            if (product != null && product.OrganizationId != Guid.Empty)
            {
                batch.ChangeOrganization(product.OrganizationId);

                if (!string.IsNullOrWhiteSpace(product.Gtin))
                {
                    var prodDate = command.ProductionDate.ToString("yyMMdd");
                    var expDate = command.ExpiryDate.HasValue ? $"(17){command.ExpiryDate.Value.ToString("yyMMdd")}" : "";
                    var gs1Code = $"(01){product.Gtin}(10){batchCode}(11){prodDate}{expDate}";
                    batch.SetGs1BatchCode(gs1Code);
                }
            }
            else
            {
                batch.ChangeOrganization(new Guid("50000000-0000-0000-0000-000000000001"));
            }
        }
        else
        {
            batch.ChangeOrganization(new Guid("50000000-0000-0000-0000-000000000001"));
        }

        var created = await _batchWriteService.CreateAsync(
            batch,
            cancellationToken);

        if (_eventTypeRepository != null && _eventService != null)
        {
            var eventType = await _eventTypeRepository.GetByCodeAsync("CREATED", cancellationToken);
            if (eventType == null)
            {
                var newEventType = new EventType(
                    "CREATED",
                    "Created");
                try
                {
                    eventType = await _eventTypeRepository.AddAsync(newEventType, cancellationToken);
                }
                catch
                {
                    eventType = await _eventTypeRepository.GetByCodeAsync("HARVEST", cancellationToken);
                }
            }
            if (eventType != null)
            {
                var performedByUserId = _currentUser?.UserId ?? Guid.Empty;
                if (performedByUserId == Guid.Empty)
                {
                    performedByUserId = new Guid("70000000-0000-0000-0000-000000000002");
                }

                var supplyChainEvent = new SupplyChainEvent(
                    created.Id,
                    eventType.Id,
                    batch.CurrentOrganizationId,
                    performedByUserId,
                    eventData: $"Lô hàng {batchCode} được khởi tạo. Sản lượng: {command.Quantity}",
                    location: !string.IsNullOrWhiteSpace(command.Location) ? command.Location : "Nông trại sản xuất",
                    inspectionId: null,
                    previousHash: null,
                    currentHash: null);

                await _eventService.CreateEventAsync(supplyChainEvent, cancellationToken);
            }
        }

        if (_notificationService != null && _userService != null && batch.CurrentOrganizationId != Guid.Empty)
        {
            var orgUsers = await _userService.GetByOrganizationAsync(batch.CurrentOrganizationId, cancellationToken);
            foreach (var u in orgUsers)
            {
                var notif = new Notification(
                    u.Id,
                    "📦 LÔ HÀNG MỚI ĐƯỢC KHỞI TẠO",
                    $"Lô hàng {batchCode} (Sản lượng: {command.Quantity}) vừa được khởi tạo thành công trên hệ thống.",
                    "BATCH");
                await _notificationService.CreateAsync(notif, cancellationToken);
            }
        }

        return created.Adapt<BatchDto>();
    }

}




public sealed class CreateBatchCommandValidator
    : AbstractValidator<CreateBatchCommand>
{

    public CreateBatchCommandValidator()
    {

        RuleFor(x => x.ProductId)
            .NotEmpty();



        RuleFor(x => x.UnitId)
            .NotEmpty();



        RuleFor(x => x.Quantity)
            .GreaterThan(0);



        RuleFor(x => x.ProductionDate)
            .NotEmpty();



        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(x => x.ProductionDate)
            .WithMessage("Hạn sử dụng (Expiry Date) phải lớn hơn hoặc bằng Ngày sản xuất.")
            .When(x => x.ExpiryDate.HasValue);

    }

}
