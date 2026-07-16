namespace AgriTrace.Domain.Common;

/// <summary>
/// Thrown when a requested resource cannot be found.
/// Mapped to HTTP 404 by GlobalExceptionHandler.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
