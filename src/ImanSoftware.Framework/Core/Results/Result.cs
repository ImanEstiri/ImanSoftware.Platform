
using System.Diagnostics.CodeAnalysis;

namespace ImanSoftware.Framework.Core.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public AppError Error { get; }

    public Result(bool isSuccess, AppError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Successful() => new(true, AppError.None);
    public static Result<T> Successful<T>(T data) => new(data, true, AppError.None);

    public static Result Failure(AppError error) => new(false, error);
    public static Result<T> Failure<T>(AppError error) => new(default, false, error);

}


public class Result<T> : Result
{
    private readonly T? _data;
    public Result(T? data, bool isSuccess, AppError error) : base(isSuccess, error)
    {
        _data = data;
    }

    [NotNull]
    public T Data => IsSuccess && _data is not null ? _data! : throw new InvalidOperationException("Cannot access Data of a failed result. Check IsSuccess first.");


    public static implicit operator Result<T>(T? _data)
        => _data is not null ?
            Successful(_data) : Failure<T>(AppError.Create("", "", ErrorType.Problem));
}
