using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Queries;


public sealed record GetBatchesQuery(
    Guid? ProductId,
    Guid? OrganizationId,
    string? Search,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<BatchDto>>;




public sealed class GetBatchesQueryHandler
    : IRequestHandler<GetBatchesQuery, PagedResult<BatchDto>>
{

    private readonly IBatchReadService _batchReadService;



    public GetBatchesQueryHandler(
        IBatchReadService batchReadService)
    {
        _batchReadService = batchReadService;
    }



    public async Task<PagedResult<BatchDto>> Handle(
        GetBatchesQuery request,
        CancellationToken cancellationToken)
    {

        var pagedBatches = await _batchReadService.SearchAsync(
            request.ProductId,
            request.OrganizationId,
            request.Search,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoItems = pagedBatches.Items
            .Select(b => b.Adapt<BatchDto>())
            .ToList();

        return new PagedResult<BatchDto>(
            dtoItems,
            pagedBatches.TotalCount,
            pagedBatches.PageNumber,
            pagedBatches.PageSize);

    }

}