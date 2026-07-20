using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Lookup.Queries;

public record GetCertificateTypesQuery : IRequest<IReadOnlyList<LookupItemDto>>;

/// <summary>
/// Certificate types have no dedicated table, so the list is derived from a well-known base set
/// merged with the distinct <c>CertificateType</c> values already stored on certificates.
/// </summary>
public class GetCertificateTypesQueryHandler : IRequestHandler<GetCertificateTypesQuery, IReadOnlyList<LookupItemDto>>
{
    private static readonly string[] KnownTypes =
    {
        "VietGAP",
        "GlobalGAP",
        "Organic",
        "HACCP",
        "ISO22000"
    };

    private readonly ICertificateService _certificateService;

    public GetCertificateTypesQueryHandler(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    public async Task<IReadOnlyList<LookupItemDto>> Handle(GetCertificateTypesQuery request, CancellationToken cancellationToken)
    {
        var stored = (await _certificateService.GetAllAsync(cancellationToken))
            .Select(c => c.CertificateType)
            .Where(t => !string.IsNullOrWhiteSpace(t));

        var names = KnownTypes
            .Concat(stored)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();

        return names
            .Select(name => new LookupItemDto
            {
                Id = name,
                Code = name.ToUpperInvariant(),
                Name = name
            })
            .ToList();
    }
}
