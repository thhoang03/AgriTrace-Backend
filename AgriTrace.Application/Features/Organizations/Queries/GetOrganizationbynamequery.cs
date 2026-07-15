using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

// Trả về null nếu không tìm thấy (khác GetOrganizationByIdQuery) vì query này
// thường được dùng để kiểm tra tồn tại / trùng tên, không phải để lấy chi tiết theo khóa chính.
public record GetOrganizationByNameQuery(string Name) : IRequest<OrganizationDto?>;

public class GetOrganizationByNameQueryHandler : IRequestHandler<GetOrganizationByNameQuery, OrganizationDto?>
{
    private readonly IOrganizationService _organizationService;

    public GetOrganizationByNameQueryHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<OrganizationDto?> Handle(GetOrganizationByNameQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByNameAsync(request.Name, cancellationToken);

        return organization?.Adapt<OrganizationDto>();
    }
}