using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record DeleteOrganizationCommand(Guid Id) : IRequest;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
{
    private readonly IOrganizationService _organizationService;

    public DeleteOrganizationCommandHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        await _organizationService.DeleteAsync(organization.Id, cancellationToken);
    }
}
