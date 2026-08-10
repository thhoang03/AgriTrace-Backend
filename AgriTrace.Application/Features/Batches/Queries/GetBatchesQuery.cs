using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Batches.Queries;

public sealed record GetBatchesQuery(
    Guid? ProductId,
    Guid? OrganizationId,
    string? Search,
    int Page,
    int PageSize)
    : IRequest<PagedResult<BatchDto>>;

public sealed class GetBatchesQueryHandler
    : IRequestHandler<GetBatchesQuery, PagedResult<BatchDto>>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IUnitRepository? _unitRepository;
    private readonly IProductReadService? _productReadService;
    private readonly IOrganizationRepository? _organizationRepository;
    private readonly IEventService? _eventService;
    private readonly IUserService? _userService;

    public GetBatchesQueryHandler(
        IBatchReadService batchReadService,
        IUnitRepository? unitRepository = null,
        IProductReadService? productReadService = null,
        IOrganizationRepository? organizationRepository = null,
        IEventService? eventService = null,
        IUserService? userService = null)
    {
        _batchReadService = batchReadService;
        _unitRepository = unitRepository;
        _productReadService = productReadService;
        _organizationRepository = organizationRepository;
        _eventService = eventService;
        _userService = userService;
    }

    public async Task<PagedResult<BatchDto>> Handle(
        GetBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var pagedBatches = await _batchReadService.SearchAsync(
            request.ProductId,
            request.OrganizationId,
            request.Search,
            request.Page,
            request.PageSize,
            cancellationToken);

        Dictionary<Guid, string>? unitCodes = null;
        if (_unitRepository != null)
        {
            var units = await _unitRepository.GetAllAsync(cancellationToken);
            unitCodes = units.ToDictionary(u => u.Id, u => u.Code);
        }

        Dictionary<Guid, string>? products = null;
        if (_productReadService != null)
        {
            var prods = await _productReadService.GetAllAsync(cancellationToken);
            products = prods.ToDictionary(p => p.Id, p => p.Name);
        }

        Dictionary<Guid, string>? orgNames = null;
        if (_organizationRepository != null)
        {
            var orgs = await _organizationRepository.GetAllAsync(cancellationToken);
            orgNames = orgs.ToDictionary(o => o.Id, o => o.Name);
        }

        var dtoItems = new List<BatchDto>();

        foreach (var b in pagedBatches.Items)
        {
            var dto = b.Adapt<BatchDto>();
            dto.CurrentOrganizationId = b.CurrentOrganizationId;
            dto.QrCodeUrl = b.QRCode;
            dto.ProductGtin = b.Product?.Gtin;

            if (unitCodes != null && unitCodes.TryGetValue(b.UnitId, out var uCode))
            {
                dto.UnitCode = uCode;
            }

            if (products != null && products.TryGetValue(b.ProductId, out var prodName))
            {
                dto.ProductName = prodName;
            }

            if (orgNames != null && orgNames.TryGetValue(b.CurrentOrganizationId, out var oName))
            {
                dto.OrganizationName = oName;
            }

            if (_eventService != null && _userService != null)
            {
                var events = await _eventService.GetByBatchAsync(b.Id, cancellationToken);
                var firstEvent = events.OrderBy(e => e.CreatedAt).FirstOrDefault();
                if (firstEvent != null)
                {
                    var user = await _userService.GetByIdAsync(firstEvent.PerformedByUserId, cancellationToken);
                    dto.FarmerName = user?.FullName ?? user?.Email;
                }
            }

            dtoItems.Add(dto);
        }

        return new PagedResult<BatchDto>(
            dtoItems,
            pagedBatches.TotalCount,
            pagedBatches.PageNumber,
            pagedBatches.PageSize);
    }
}