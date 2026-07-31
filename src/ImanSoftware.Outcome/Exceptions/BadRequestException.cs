


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown for general bad requests (HTTP 400).
/// </summary>
public class BadRequestException : FrameworkException
{
    public BadRequestException(string message)
        : base(message, "BAD_REQUEST", OutcomeErrorType.Failure)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, "BAD_REQUEST", OutcomeErrorType.Failure, innerException)
    {
    }
}