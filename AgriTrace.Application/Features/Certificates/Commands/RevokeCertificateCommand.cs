using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Certificates.Commands;

public record RevokeCertificateCommand(Guid Id) : IRequest;

public sealed class RevokeCertificateCommandHandler
    : IRequestHandler<RevokeCertificateCommand>
{
    private readonly ICertificateService _service;

    public RevokeCertificateCommandHandler(
        ICertificateService service)
    {
        _service = service;
    }

    public async Task Handle(
        RevokeCertificateCommand request,
        CancellationToken cancellationToken)
    {
        var certificate = await _service.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Certificate {request.Id} not found.");

        await _service.DeleteAsync(certificate.Id, cancellationToken);
    }
}

public sealed class RevokeCertificateCommandValidator
    : AbstractValidator<RevokeCertificateCommand>
{
    public RevokeCertificateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Certificate Id is required.");
    }
}
