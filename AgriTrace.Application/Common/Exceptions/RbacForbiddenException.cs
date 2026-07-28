namespace AgriTrace.Application.Common.Exceptions;

public class RbacForbiddenException : Exception
{
    public string Code { get; }
    public string? Details { get; }

    public RbacForbiddenException(string code, string message, string? details = null)
        : base(message)
    {
        Code = code;
        Details = details;
    }
}
