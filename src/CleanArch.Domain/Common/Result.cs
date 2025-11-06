namespace CleanArch.Domain.Common;

/// <summary>
/// Representa el resultado de una operación del dominio
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, string.Empty);
    public static Result<TValue> Failure<TValue>(string error) => new(default, false, error);
}

/// <summary>
/// Resultado con valor de retorno
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access value of a failed result. Error: {Error}");

    public new static Result<TValue> Success(TValue value) => new(value, true, string.Empty);
    public new static Result<TValue> Failure(string error) => new(default, false, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
