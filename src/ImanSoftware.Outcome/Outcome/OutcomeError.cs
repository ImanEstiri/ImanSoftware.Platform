

namespace ImanSoftware.Outcomes;

public record OutcomeError
{
    public string Message { get; }
    public string Code { get; } = string.Empty;
    public OutcomeErrorType Type { get; } = OutcomeErrorType.Failure;

    public OutcomeError(string message, string code, OutcomeErrorType type)
    {
        Message = message;
        Code = code;
        Type = type;
    }

    public OutcomeError(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Message = exception.Message;
        Code = string.Empty;
    }

    public static OutcomeError DefaultError => new("Somthing went Wrong . . .", "", OutcomeErrorType.Failure);
    public static OutcomeError None => new(string.Empty, string.Empty, OutcomeErrorType.None);
    public static OutcomeError Create(string msg, string code, OutcomeErrorType type) => new(msg, code, type);
}
