using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class User : BaseEntity
{
    public Guid? OrganizationId { get; private set; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string Role { get; private set; }

    public bool IsActive { get; private set; }

    // Navigation

    public Organization? Organization { get; private set; }

    private User()
    {
    }

    public User(
        Guid? organizationId,
        string fullName,
        string email,
        string passwordHash,
        string role)
    {
        Validate(
            fullName,
            email,
            passwordHash,
            role);

        OrganizationId = organizationId;
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role.Trim().ToUpperInvariant();
        IsActive = true;
    }

    public void UpdateProfile(
        string fullName,
        string email,
        string role)
    {
        Validate(
            fullName,
            email,
            PasswordHash,
            role);

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = role.Trim().ToUpperInvariant();

        MarkUpdated();
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.");
        }

        PasswordHash = passwordHash;

        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;

        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;

        MarkUpdated();
    }

    private static void Validate(
        string fullName,
        string email,
        string passwordHash,
        string role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("Email is invalid.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.");
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role is required.");
        }
    }
}