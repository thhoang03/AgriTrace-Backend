using AgriTrace.Domain.Enums;

namespace AgriTrace.Application.Contracts;

public class UserDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string OrganizationTypeName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public UserStatus Status { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime CreatedAt { get; set; }
}
