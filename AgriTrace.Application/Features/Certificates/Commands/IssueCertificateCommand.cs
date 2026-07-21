using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Certificates.Commands;

public sealed record IssueCertificateCommand(
    Guid BatchId,
    Guid InspectionId,
    string CertificateType,
    string FileUrl,
    DateOnly IssuedDate)
    : IRequest<CertificateDto>;

public sealed class IssueCertificateCommandHandler
    : IRequestHandler<IssueCertificateCommand, CertificateDto>
{
    private readonly ICertificateService _service;

    public IssueCertificateCommandHandler(
        ICertificateService service)
    {
        _service = service;
    }

    public async Task<CertificateDto> Handle(
        IssueCertificateCommand command,
        CancellationToken cancellationToken)
    {
        var certificate = new Certificate(
            command.BatchId,
            command.InspectionId,
            command.CertificateType,
            command.FileUrl,
            command.IssuedDate.ToDateTime(TimeOnly.MinValue));

        var created = await _service.CreateAsync(certificate, cancellationToken);

        return new CertificateDto
        {
            Id = created.Id,
            BatchId = created.BatchId,
            InspectionId = created.InspectionId,
            CertificateType = created.CertificateType,
            FileUrl = created.FileUrl,
            IssuedDate = created.IssuedDate,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}

public sealed class IssueCertificateCommandValidator
    : AbstractValidator<IssueCertificateCommand>
{
    public IssueCertificateCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("BatchId is required.");

        RuleFor(x => x.InspectionId)
            .NotEmpty()
            .WithMessage("InspectionId is required.");

        RuleFor(x => x.CertificateType)
            .NotEmpty()
            .WithMessage("CertificateType is required.");

        RuleFor(x => x.FileUrl)
            .NotEmpty()
            .WithMessage("FileUrl is required.");

        RuleFor(x => x.IssuedDate)
            .NotEmpty()
            .WithMessage("IssuedDate is required.");
    }
}
