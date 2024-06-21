namespace BuildingBlocks.Common;

public interface IAppResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    Error Error { get; }
}

public class Result : IAppResult
{
    internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Ok() => new(true, Error.None);
    public static Result Fail(Error error) => new(false, error);
    
    public static implicit operator Result(Error error) => Fail(error);
}

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);

    public static implicit operator Result(Error error) => Result.Fail(error);
}

public class Result<T> : Result
{
    private Result(bool isSuccess, Error error, T value) : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; set; }

    public static Result<T> Ok(T value) => new(true, Error.None, value);
    
    public new static Result<T> Fail(Error error) => new(false, error, default!);
    
    public static Result<T> Fail(string error) => new(false, new Error(error), default!);
    
    public static implicit operator Result<T>(T value) => Ok(value);
}