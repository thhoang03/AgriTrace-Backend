using AgriTrace.Domain.Common;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Commands;


public sealed record UpdateBatchCommand(
    Guid Id,
    decimal? Quantity,
    DateOnly? ExpiryDate)
    : IRequest<BatchDto>;




public sealed class UpdateBatchCommandHandler
    : IRequestHandler<UpdateBatchCommand, BatchDto>
{

    private readonly IBatchReadService _batchReadService;

    private readonly IBatchWriteService _batchWriteService;



    public UpdateBatchCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService)
    {
        _batchReadService = batchReadService;

        _batchWriteService = batchWriteService;
    }



    public async Task<BatchDto> Handle(
        UpdateBatchCommand command,
        CancellationToken cancellationToken)
    {

        var batch =
            await _batchReadService.GetByIdAsync(
                command.Id,
                cancellationToken);



        if (batch == null)
        {
            throw new AgriTrace.Application.Common.Exceptions.NotFoundException("Batch not found.");
        }



        // Only quantity and expiryDate are updatable; preserve existing batch code and production date.
        var quantity =
            command.Quantity ?? batch.Quantity;

        DateTime? expiryDate =
            command.ExpiryDate.HasValue
                ? command.ExpiryDate.Value.ToDateTime(TimeOnly.MinValue)
                : batch.ExpiryDate;



        batch.UpdateInformation(
            batch.BatchCode,
            quantity,
            batch.ProductionDate,
            expiryDate);



        await _batchWriteService.UpdateAsync(
            batch,
            cancellationToken);



        return batch.Adapt<BatchDto>();

    }

}




public sealed class UpdateBatchCommandValidator
    : AbstractValidator<UpdateBatchCommand>
{

    public UpdateBatchCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty();


        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue);

    }

}