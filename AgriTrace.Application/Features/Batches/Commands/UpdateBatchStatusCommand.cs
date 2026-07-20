using AgriTrace.Domain.Common;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Batches.Commands;

public sealed record UpdateBatchStatusCommand(
    Guid BatchId,
    int Status) : IRequest<BatchDto>;

public sealed class UpdateBatchStatusCommandHandler
    : IRequestHandler<UpdateBatchStatusCommand, BatchDto>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IBatchWriteService _batchWriteService;

    public UpdateBatchStatusCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService)
    {
        _batchReadService = batchReadService;
        _batchWriteService = batchWriteService;
    }

    public async Task<BatchDto> Handle(
        UpdateBatchStatusCommand command,
        CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(command.BatchId, cancellationToken)
            ?? throw new NotFoundException("Batch not found.");

        batch.ChangeStatus((BatchStatus)command.Status);

        await _batchWriteService.UpdateAsync(batch, cancellationToken);

        return batch.Adapt<BatchDto>();
    }
}
