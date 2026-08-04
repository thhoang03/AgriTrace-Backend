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
    DateTime? ExpiryDate)
    : IRequest<BatchDto>;



public sealed class CreateBatchCommandHandler
    : IRequestHandler<CreateBatchCommand, BatchDto>
{

    private readonly IBatchWriteService _batchWriteService;
    private readonly IProductReadService? _productReadService;
    private readonly IEventTypeRepository? _eventTypeRepository;
    private readonly IEventService? _eventService;
    private readonly ICurrentUserService? _currentUser;

    public CreateBatchCommandHandler(
        IBatchWriteService batchWriteService,
        IProductReadService? productReadService = null,
        IEventTypeRepository? eventTypeRepository = null,
        IEventService? eventService = null,
        ICurrentUserService? currentUser = null)
    {
        _batchWriteService = batchWriteService;
        _productReadService = productReadService;
        _eventTypeRepository = eventTypeRepository;
        _eventService = eventService;
        _currentUser = currentUser;
    }

    public async Task<BatchDto> Handle(
        CreateBatchCommand command,
        CancellationToken cancellationToken)
    {
        if (_currentUser != null && _currentUser.IsAuthenticated && _currentUser.Role != "Admin" && _currentUser.OrganizationType != "FARM")
        {
            throw new ForbiddenException("Chỉ đơn vị Trang trại (FARM) hoặc Admin hệ thống mới được phép khởi tạo Lô hàng mới.");
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
            .GreaterThan(x => x.ProductionDate)
            .When(x => x.ExpiryDate.HasValue);

    }

}
