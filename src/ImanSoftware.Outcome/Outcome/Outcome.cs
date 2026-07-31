
using System.Diagnostics.CodeAnalysis;

namespace ImanSoftware.Outcomes;

public class Outcome
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public OutcomeError Error { get; }

    public Outcome(bool isSuccess, OutcomeError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Outcome Successful() => new(true, OutcomeError.None);
    public static Outcome<T> Successful<T>(T data) => new(data, true, OutcomeError.None);

    public static Outcome Failure(OutcomeError error) => new(false, error);
    public static Outcome<T> Failure<T>(OutcomeError error) => new(default, false, error);

}


public class Outcome<T> : Outcome
{
    private readonly T? _data;
    public Outcome(T? data, bool isSuccess, OutcomeError error) : base(isSuccess, error)
    {
        _data = data;
    }

    [NotNull]
    public T Data => IsSuccess && _data is not null ? _data! : throw new InvalidOperationException("Cannot access Data of a failed result. Check IsSuccess first.");


    public static implicit operator Outcome<T>(T? _data)
        => _data is not null ?
            Successful(_data) : Failure<T>(OutcomeError.Create("", "", OutcomeErrorType.Problem));
}
