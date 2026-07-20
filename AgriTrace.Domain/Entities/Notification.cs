using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }

    public string Title { get; private set; }

    public string Message { get; private set; }

    public bool IsRead { get; private set; }

    // Navigation

    public User User { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid userId,
        string title,
        string message)
    {
        Validate(userId, title, message);

        UserId = userId;
        Title = title.Trim();
        Message = message.Trim();
        IsRead = false;
    }

    /// <summary>
    /// Rehydrates a Notification from persisted storage. Infrastructure use only.
    /// </summary>
    public static Notification Rehydrate(
        Guid id,
        Guid userId,
        string title,
        string message,
        bool isRead,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Notification
        {
            Id = id,
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = isRead,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;

        MarkUpdated();
    }

    public void UpdateContent(
        string title,
        string message)
    {
        ValidateContent(title, message);

        Title = title.Trim();
        Message = message.Trim();

        MarkUpdated();
    }

    private static void Validate(
        Guid userId,
        string title,
        string message)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User is required.");
        }

        ValidateContent(title, message);
    }

    private static void ValidateContent(
        string title,
        string message)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Notification title is required.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Notification message is required.");
        }
    }
}