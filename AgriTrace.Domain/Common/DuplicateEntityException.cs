namespace AgriTrace.Domain.Common;

public class DuplicateEntityException : Exception
{
    public DuplicateEntityException(string message) : base(message)
    {
    }
}
