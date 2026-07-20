
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when the user is not authenticated (HTTP 401).
/// </summary>
public class UnauthorizedException : FrameworkException
{
    public UnauthorizedException(string message = "User is not authenticated.")
        : base(message, "UNAUTHORIZED", ErrorType.Failure)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, "UNAUTHORIZED", ErrorType.Failure, innerException)
    {
    }
}