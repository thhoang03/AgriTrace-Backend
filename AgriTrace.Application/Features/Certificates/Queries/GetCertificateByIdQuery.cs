using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Certificates.Queries;

public sealed record GetCertificateByIdQuery(Guid Id)
    : IRequest<CertificateDto?>;

public sealed class GetCertificateByIdQueryHandler
    : IRequestHandler<GetCertificateByIdQuery, CertificateDto?>
{
    private readonly ICertificateService _service;

    public GetCertificateByIdQueryHandler(
        ICertificateService service)
    {
        _service = service;
    }

    public async Task<CertificateDto?> Handle(
        GetCertificateByIdQuery query,
        CancellationToken cancellationToken)
    {
        var certificate = await _service.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (certificate is null)
        {
            return null;
        }

        return new CertificateDto
        {
            Id = certificate.Id,
            BatchId = certificate.BatchId,
            InspectionId = certificate.InspectionId,
            CertificateNumber = certificate.CertificateNumber,
            CertificateType = certificate.CertificateType,
            IssuingOrganization = certificate.IssuingOrganization,
            FileUrl = certificate.FileUrl,
            IssuedDate = certificate.IssuedDate,
            ExpiryDate = certificate.ExpiryDate,
            Status = (int)certificate.Status,
            StatusName = certificate.Status.ToString(),
            Notes = certificate.Notes,
            CreatedAt = certificate.CreatedAt,
            UpdatedAt = certificate.UpdatedAt
        };
    }
}

