using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record ApproveOrganizationCommand(Guid Id) : IRequest<OrganizationDto>;

public class ApproveOrganizationCommandHandler : IRequestHandler<ApproveOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;
    private readonly IUserService _userService;

    public ApproveOrganizationCommandHandler(
        IOrganizationService organizationService,
        IUserService userService)
    {
        _organizationService = organizationService;
        _userService = userService;
    }

    public async Task<OrganizationDto> Handle(ApproveOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        if (organization.Status != OrganizationStatus.Pending)
            throw new ArgumentException("Only pending organizations can be approved.");

        // Activate organization
        organization.SetStatus(OrganizationStatus.Active);
        await _organizationService.UpdateAsync(organization, cancellationToken);

        // Activate users belonging to organization
        var users = await _userService.GetByOrganizationAsync(request.Id, cancellationToken);
        foreach (var user in users)
        {
            if (user.Status == UserStatus.Pending)
            {
                user.Activate(); // Activate sets Status = Active and IsActive = true
                await _userService.UpdateAsync(user, cancellationToken);
            }
        }

        return organization.Adapt<OrganizationDto>();
    }
}
