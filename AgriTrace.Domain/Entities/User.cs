using System.Security.Cryptography;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Domain.Entities;

public class User : BaseEntity
{
    public Guid? OrganizationId { get; private set; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string? Phone { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    // Refresh token storage (Phase 7)
    public string? RefreshToken { get; private set; }

    public DateTime? RefreshTokenExpiry { get; private set; }

    // Password reset (Phase 7 stub)
    public string? ResetPasswordToken { get; private set; }

    public DateTime? ResetPasswordTokenExpiry { get; private set; }

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
        UserRole role)
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
        Role = role;
        IsActive = true;
    }

    public void UpdateProfile(
        string fullName,
        string email,
        UserRole role)
    {
        Validate(
            fullName,
            email,
            PasswordHash,
            role);

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = role;

        MarkUpdated();
    }

    /// <summary>
    /// Partial profile update used by the Users feature (only non-null fields are applied).
    /// </summary>
    public void UpdateProfileDetails(
        string? fullName,
        string? phone,
        UserRole? role)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName.Trim();
        }

        if (phone != null)
        {
            Phone = phone.Trim();
        }

        if (role.HasValue)
        {
            if (!Enum.IsDefined(typeof(UserRole), role.Value))
            {
                throw new ArgumentException("Role is invalid.");
            }

            Role = role.Value;
        }

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

    public void SetChangeStatus(bool isActive)
    {
        IsActive = isActive;
        MarkUpdated();
    }

    // ---- Refresh token ----

    public void SetRefreshToken(string refreshToken, DateTime expiry)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = expiry;
        MarkUpdated();
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
        MarkUpdated();
    }

    public bool IsRefreshTokenValid(string refreshToken)
        => RefreshToken == refreshToken
           && RefreshTokenExpiry.HasValue
           && RefreshTokenExpiry.Value > DateTime.UtcNow;

    // ---- Password reset ----

    public void SetResetPasswordToken(string token, DateTime expiry)
    {
        ResetPasswordToken = token;
        ResetPasswordTokenExpiry = expiry;
        MarkUpdated();
    }

    public bool IsResetPasswordTokenValid(string token)
        => ResetPasswordToken == token
           && ResetPasswordTokenExpiry.HasValue
           && ResetPasswordTokenExpiry.Value > DateTime.UtcNow;

    public void ClearResetPasswordToken()
    {
        ResetPasswordToken = null;
        ResetPasswordTokenExpiry = null;
        MarkUpdated();
    }

    /// <summary>
    /// Rehydrates a User from persisted storage. Used only by the infrastructure layer.
    /// </summary>
    public static User Rehydrate(
        Guid id,
        Guid? organizationId,
        string fullName,
        string email,
        string passwordHash,
        string? phone,
        UserRole role,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt,
        string? refreshToken,
        DateTime? refreshTokenExpiry,
        string? resetPasswordToken,
        DateTime? resetPasswordTokenExpiry)
    {
        var user = new User(
            organizationId,
            fullName,
            email,
            passwordHash,
            role)
        {
            Id = id,
            Phone = phone,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry,
            ResetPasswordToken = resetPasswordToken,
            ResetPasswordTokenExpiry = resetPasswordTokenExpiry
        };

        return user;
    }

    // ---- Password hashing (PBKDF2 / SHA256) ----

    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    /// <summary>
    /// Hashes a plaintext password using PBKDF2 (SHA256) and stores it. Format: {iterations}.{saltB64}.{keyB64}.
    /// </summary>
    public void SetPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("Password is required.");
        }

        PasswordHash = HashPassword(plainPassword);
        MarkUpdated();
    }

    /// <summary>
    /// Verifies a plaintext password against the stored PBKDF2 hash.
    /// </summary>
    public bool VerifyPassword(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(PasswordHash))
        {
            return false;
        }

        var parts = PasswordHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expectedKey = Convert.FromBase64String(parts[2]);

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            plainPassword,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    public static string HashPassword(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            plainPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    private static void Validate(
        string fullName,
        string email,
        string passwordHash,
        UserRole role)
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

        if (!Enum.IsDefined(typeof(UserRole), role))
        {
            throw new ArgumentException("Role is invalid.");
        }
    }
}
