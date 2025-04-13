using FluentValidation.Results;

namespace CafeEmployeeApi.Contracts;

public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error == null;
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string error) => new(default, error);
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }
    public bool IsValid => ValidationErrors == null;
    private Result(T? value, string? error)
    {
        Value = value;
        Error = error;
    }

    public Result(ValidationResult validationResult)
    {
        ValidationErrors = validationResult.ToDictionary();
    }
}