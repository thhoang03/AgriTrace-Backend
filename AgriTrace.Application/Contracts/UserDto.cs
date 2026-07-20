using System;

namespace AgriTrace.Application.Contracts;

/// <summary>
/// Stub DTO for a user list item. Matches swagger <c>UserListItem</c>.
/// NOTE: Only the fields needed for the Phase 6 controller stub are populated; the Users
/// feature (Phase 7) will flesh out the handler and fill these properties.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
