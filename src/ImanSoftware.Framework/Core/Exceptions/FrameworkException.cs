using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Core.Exceptions;

public abstract class FrameworkException : Exception
{
    public string ErrorCode { get; }
    public ErrorType Type { get; }

    protected FrameworkException(string message, string errorCode, ErrorType errorType)
        : base(message)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    protected FrameworkException(string message, string errorCode, ErrorType errorType, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    /// <summary>
    /// Converts the exception to a Result object (without data).
    /// </summary>
    public Result ToResult()
    {
        return Result.Failure(new AppError(Message, ErrorCode, Type));
    }

    /// <summary>
    /// Converts the exception to a Result object with data.
    /// </summary>
    public Result<T> ToResult<T>()
    {
        return Result.Failure<T>(new AppError(Message, ErrorCode, Type));
    }
}