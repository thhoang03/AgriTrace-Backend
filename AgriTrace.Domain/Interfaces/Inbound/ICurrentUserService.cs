namespace AgriTrace.Domain.Interfaces.Inbound;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Role { get; }
    Guid? OrganizationId { get; }
    string? OrganizationType { get; }
    bool IsAuthenticated { get; }
}
