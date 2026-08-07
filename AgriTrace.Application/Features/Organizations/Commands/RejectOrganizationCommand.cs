using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record RejectOrganizationCommand(Guid Id, string? Reason) : IRequest<OrganizationDto>;

public class RejectOrganizationCommandHandler : IRequestHandler<RejectOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;
    private readonly IUserService _userService;

    public RejectOrganizationCommandHandler(
        IOrganizationService organizationService,
        IUserService userService)
    {
        _organizationService = organizationService;
        _userService = userService;
    }

    public async Task<OrganizationDto> Handle(RejectOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        if (organization.Status != OrganizationStatus.Pending)
            throw new ArgumentException("Only pending organizations can be rejected.");

        // Reject organization
        organization.SetStatus(OrganizationStatus.Rejected);
        await _organizationService.UpdateAsync(organization, cancellationToken);

        // Reject users belonging to organization
        var users = await _userService.GetByOrganizationAsync(request.Id, cancellationToken);
        foreach (var user in users)
        {
            if (user.Status == UserStatus.Pending)
            {
                user.Reject();
                await _userService.UpdateAsync(user, cancellationToken);
            }
        }

        return organization.Adapt<OrganizationDto>();
    }
}
