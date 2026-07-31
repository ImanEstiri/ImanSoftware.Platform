


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when the user is not authenticated (HTTP 401).
/// </summary>
public class UnauthorizedException : FrameworkException
{
    public UnauthorizedException(string message = "User is not authenticated.")
        : base(message, "UNAUTHORIZED", OutcomeErrorType.Failure)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, "UNAUTHORIZED", OutcomeErrorType.Failure, innerException)
    {
    }
}