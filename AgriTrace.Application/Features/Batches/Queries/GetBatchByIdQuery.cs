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



    public GetBatchByIdQueryHandler(
        IBatchReadService batchReadService)
    {
        _batchReadService = batchReadService;
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

        return batch.Adapt<BatchDto>();

    }

}