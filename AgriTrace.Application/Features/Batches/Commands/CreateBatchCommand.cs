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

        // Server-side batch code generation (placeholder until a proper sequence service is built).
        var batchCode = Guid.NewGuid().ToString("N")[..8].ToUpper();



        var batch = new Batch(
            command.ProductId,
            batchCode,
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



        RuleFor(x => x.Quantity)
            .GreaterThan(0);



        RuleFor(x => x.ProductionDate)
            .NotEmpty();



        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.ProductionDate)
            .When(x => x.ExpiryDate.HasValue);

    }

}