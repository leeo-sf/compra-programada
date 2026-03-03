namespace CompraProgramada.Application;

public class Result
{
    public bool IsSuccess { get; protected set; } = default!;
    public Exception? Exception { get; protected set; } = default!;

    public static Result Ok() =>
        new Result { IsSuccess = true };

    public static Result Fail(Exception ex) =>
        new Result { IsSuccess = false, Exception = ex };
}

public class Result<T> : Result
{
    public T? Value { get; set; }

    public static Result<T> Ok(T value) =>
        new Result<T> { IsSuccess = true, Value = value };

    public new static Result<T> Fail(Exception? ex = null) =>
        new Result<T> { IsSuccess = false, Exception = ex };
}