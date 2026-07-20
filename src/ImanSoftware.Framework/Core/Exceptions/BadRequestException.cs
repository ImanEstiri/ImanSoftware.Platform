
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown for general bad requests (HTTP 400).
/// </summary>
public class BadRequestException : FrameworkException
{
    public BadRequestException(string message)
        : base(message, "BAD_REQUEST", ErrorType.Failure)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, "BAD_REQUEST", ErrorType.Failure, innerException)
    {
    }
}