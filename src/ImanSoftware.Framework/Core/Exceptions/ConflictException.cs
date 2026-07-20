
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (HTTP 409).
/// </summary>
public class ConflictException : FrameworkException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", ErrorType.Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, "CONFLICT", ErrorType.Conflict, innerException)
    {
    }
}