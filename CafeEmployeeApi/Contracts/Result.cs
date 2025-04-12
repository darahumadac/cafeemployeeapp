namespace CafeEmployeeApi.Contracts;

public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error == null;

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string error) => new(default, error);

    private Result(T? value, string? error)
    {
        Value = value;
        Error = error;
    }
}