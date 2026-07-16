using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;


namespace AgriTrace.Application.Features.Batches.Commands;


public sealed record DeleteBatchCommand(
    Guid Id)
    : IRequest;


public sealed class DeleteBatchCommandHandler
    : IRequestHandler<DeleteBatchCommand>
{

    private readonly IBatchReadService _batchReadService;

    private readonly IBatchWriteService _batchWriteService;



    public DeleteBatchCommandHandler(
        IBatchReadService batchReadService,
        IBatchWriteService batchWriteService)
    {
        _batchReadService = batchReadService;

        _batchWriteService = batchWriteService;
    }






    public async Task Handle(
        DeleteBatchCommand command,
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



        await _batchWriteService.DeleteAsync(
            command.Id,
            cancellationToken);

    }

}






public sealed class DeleteBatchCommandValidator
    : AbstractValidator<DeleteBatchCommand>
{

    public DeleteBatchCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty();

    }

}