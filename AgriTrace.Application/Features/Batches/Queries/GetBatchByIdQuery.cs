using AgriTrace.Domain.Common;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Queries;


public sealed record GetBatchByIdQuery(
    Guid Id)
    : IRequest<BatchDto>;




public sealed class GetBatchByIdQueryHandler
    : IRequestHandler<GetBatchByIdQuery, BatchDto>
{

    private readonly IBatchReadService _batchReadService;
    private readonly IEventService? _eventService;
    private readonly IUserService? _userService;

    public GetBatchByIdQueryHandler(
        IBatchReadService batchReadService,
        IEventService? eventService = null,
        IUserService? userService = null)
    {
        _batchReadService = batchReadService;
        _eventService = eventService;
        _userService = userService;
    }

    public async Task<BatchDto> Handle(
        GetBatchByIdQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (batch is null)
        {
            throw new NotFoundException("Batch not found.");
        }

        var dto = batch.Adapt<BatchDto>();
        dto.CurrentOrganizationId = batch.CurrentOrganizationId;
        dto.QrCodeUrl = batch.QRCode;
        dto.ProductGtin = batch.Product?.Gtin;

        if (_eventService != null && _userService != null)
        {
            var events = await _eventService.GetByBatchAsync(batch.Id, cancellationToken);
            var firstEvent = events.OrderBy(e => e.CreatedAt).FirstOrDefault();
            if (firstEvent != null)
            {
                var user = await _userService.GetByIdAsync(firstEvent.PerformedByUserId, cancellationToken);
                dto.FarmerName = user?.FullName ?? user?.Email;
            }
        }

        return dto;
    }

}