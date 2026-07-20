

namespace ImanSoftware.Framework.Core.Results;

public record AppError
{
    public string Message { get; }
    public string Code { get; } = string.Empty;
    public ErrorType Type { get; } = ErrorType.Failure;

    public AppError(string message, string code, ErrorType type)
    {
        Message = message;
        Code = code;
        Type = type;
    }

    public AppError(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Message = exception.Message;
        Code = string.Empty;
    }

    public static AppError DefaultError => new("Somthing went Wrong . . .", "", ErrorType.Failure);
    public static AppError None => new(string.Empty, string.Empty, ErrorType.None);
    public static AppError Create(string msg, string code, ErrorType type) => new(msg, code, type);
}
