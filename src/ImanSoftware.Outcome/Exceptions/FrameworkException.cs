

namespace ImanSoftware.Outcomes;

public abstract class FrameworkException : Exception
{
    public string ErrorCode { get; }
    public OutcomeErrorType Type { get; }

    protected FrameworkException(string message, string errorCode, OutcomeErrorType errorType)
        : base(message)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    protected FrameworkException(string message, string errorCode, OutcomeErrorType errorType, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    /// <summary>
    /// Converts the exception to a Result object (without data).
    /// </summary>
    public Outcome ToResult()
    {
        return Outcome.Failure(new OutcomeError(Message, ErrorCode, Type));
    }

    /// <summary>
    /// Converts the exception to a Result object with data.
    /// </summary>
    public Outcome<T> ToResult<T>()
    {
        return Outcome.Failure<T>(new OutcomeError(Message, ErrorCode, Type));
    }
}