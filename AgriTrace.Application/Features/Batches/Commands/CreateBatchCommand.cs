using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using Mapster;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Commands;


public sealed record CreateBatchCommand(
    Guid ProductId,
    Guid UnitId,
    string BatchCode,
    decimal Quantity,
    DateTime ProductionDate,
    DateTime? ExpiryDate)
    : IRequest<BatchDto>;



public sealed class CreateBatchCommandHandler
    : IRequestHandler<CreateBatchCommand, BatchDto>
{

    private readonly IBatchWriteService _batchWriteService;



    public CreateBatchCommandHandler(
        IBatchWriteService batchWriteService)
    {
        _batchWriteService = batchWriteService;
    }



    public async Task<BatchDto> Handle(
        CreateBatchCommand command,
        CancellationToken cancellationToken)
    {

        var batch = new Batch(
            command.ProductId,
            command.BatchCode,
            command.Quantity,
            command.UnitId,
            command.ProductionDate,
            command.ExpiryDate);



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