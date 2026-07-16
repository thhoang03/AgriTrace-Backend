using AgriTrace.Domain.Common;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Commands;


public sealed record UpdateBatchCommand(
    Guid Id,
    string BatchCode,
    decimal Quantity,
    DateTime ProductionDate,
    DateTime? ExpiryDate)
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
            throw new NotFoundException("Batch not found.");
        }



        batch.UpdateInformation(
            command.BatchCode,
            command.Quantity,
            command.ProductionDate,
            command.ExpiryDate);



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


        RuleFor(x => x.BatchCode)
            .NotEmpty()
            .MaximumLength(100);


        RuleFor(x => x.Quantity)
            .GreaterThan(0);


        RuleFor(x => x.ProductionDate)
            .NotEmpty();


        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.ProductionDate)
            .When(x => x.ExpiryDate.HasValue);

    }

}