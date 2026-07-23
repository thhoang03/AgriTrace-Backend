using AgriTrace.Domain.Common;
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

