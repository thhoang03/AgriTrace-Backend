using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Certificates.Queries;

public sealed record GetCertificatesByBatchQuery(Guid BatchId)
    : IRequest<IReadOnlyList<CertificateDto>>;

public sealed class GetCertificatesByBatchQueryHandler
    : IRequestHandler<GetCertificatesByBatchQuery, IReadOnlyList<CertificateDto>>
{
    private readonly ICertificateService _service;

    public GetCertificatesByBatchQueryHandler(
        ICertificateService service)
    {
        _service = service;
    }

    public async Task<IReadOnlyList<CertificateDto>> Handle(
        GetCertificatesByBatchQuery query,
        CancellationToken cancellationToken)
    {
        var certificates = await _service.GetByBatchAsync(
            query.BatchId,
            cancellationToken);

        return certificates
            .Select(c => new CertificateDto
            {
                Id = c.Id,
                BatchId = c.BatchId,
                InspectionId = c.InspectionId,
                CertificateNumber = c.CertificateNumber,
                CertificateType = c.CertificateType,
                IssuingOrganization = c.IssuingOrganization,
                FileUrl = c.FileUrl,
                IssuedDate = c.IssuedDate,
                ExpiryDate = c.ExpiryDate,
                Status = (int)c.Status,
                StatusName = c.Status.ToString(),
                Notes = c.Notes,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToList()
            .AsReadOnly();
    }
}

