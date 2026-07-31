


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when a conflict occurs (HTTP 409).
/// </summary>
public class ConflictException : FrameworkException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", OutcomeErrorType.Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, "CONFLICT", OutcomeErrorType.Conflict, innerException)
    {
    }
}