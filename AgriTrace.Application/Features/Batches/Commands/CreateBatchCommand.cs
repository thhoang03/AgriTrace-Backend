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

        // Auto-create initial HARVEST event
        if (_eventTypeRepository != null && _eventService != null)
        {
            try
            {
                var allTypes = await _eventTypeRepository.GetAllAsync(cancellationToken);
                var harvestType = allTypes.FirstOrDefault(e => e.Code.Equals("HARVEST", StringComparison.OrdinalIgnoreCase))
                    ?? allTypes.FirstOrDefault();

                if (harvestType != null)
                {
                    var userId = (_currentUser != null && _currentUser.IsAuthenticated) ? _currentUser.UserId : (created.CurrentOrganizationId);
                    if (userId == Guid.Empty) userId = new Guid("10000000-0000-0000-0000-000000000001");

                    var harvestEvent = new SupplyChainEvent(
                        created.Id,
                        harvestType.Id,
                        created.CurrentOrganizationId,
                        userId,
                        eventData: $"Thu hoạch nông sản ban đầu cho lô hàng {created.BatchCode}",
                        location: "Farm Location",
                        inspectionId: null,
                        previousHash: null,
                        currentHash: null);

                    await _eventService.CreateEventAsync(harvestEvent, cancellationToken);
                    created.ChangeStatus(BatchStatus.Harvested);
                    await _batchWriteService.UpdateAsync(created, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateBatchCommand] Note: Initial HARVEST event creation skipped: {ex.Message}");
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
            .GreaterThan(x => x.ProductionDate)
            .When(x => x.ExpiryDate.HasValue);

    }

}
