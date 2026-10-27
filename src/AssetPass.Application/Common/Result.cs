namespace AssetPass.Application.Common;

/// <summary>
/// Generic result wrapper for application operations.
/// Provides consistent error handling without exceptions for expected failures.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }
    public List<string> ValidationErrors { get; } = new();

    private Result(bool isSuccess, T? data, string? error, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error, string? errorCode = null) => new(false, default, error, errorCode);
    public static Result<T> ValidationFailure(List<string> errors) =>
        new(false, default, "Validation failed") { ValidationErrors = { } };

    public static Result<T> ValidationFailure(string error) =>
        new(false, default, error, "VALIDATION_ERROR");

    public static Result<T> NotFound(string message = "Resource not found") =>
        new(false, default, message, "NOT_FOUND");

    public static Result<T> Unauthorized(string message = "Unauthorized") =>
        new(false, default, message, "UNAUTHORIZED");

    public static Result<T> Forbidden(string message = "Forbidden") =>
        new(false, default, message, "FORBIDDEN");
}

/// <summary>
/// Non-generic result for operations that don't return data.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    public static Result NotFound(string message = "Resource not found") => new(false, message, "NOT_FOUND");
    public static Result Unauthorized(string message = "Unauthorized") => new(false, message, "UNAUTHORIZED");
    public static Result Forbidden(string message = "Forbidden") => new(false, message, "FORBIDDEN");
}
