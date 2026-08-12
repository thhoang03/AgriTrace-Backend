namespace AgriTrace.Application.Contracts;

/// <summary>
/// DTO for a notification. Mirrors swagger <c>NotificationItem</c>.
/// </summary>
public class NotificationDto
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    /// <summary>RECALL | INSPECTION | BATCH | CERTIFICATE | SYSTEM</summary>
    public string Type { get; set; } = "SYSTEM";

    public DateTime CreatedAt { get; set; }
}
