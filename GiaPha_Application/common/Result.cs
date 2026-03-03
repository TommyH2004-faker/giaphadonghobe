

namespace GiaPha_Application.Common;
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public ErrorType? ErrorType { get; }
    public string ErrorMessage { get; }
    public List<string> Errors { get; }

    private Result(
        bool isSuccess,
        T? data,
        ErrorType? errorType,
        string errorMessage,
        List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
        Errors = errors ?? new();
    }

    public static Result<T> Success(T data)
        => new(true, data, null, string.Empty);
    public static Result<T> Success(T data, string message)
    => new(true, data, null, message);

    public static Result<T> Failure(
        ErrorType errorType,
        string message,
        List<string>? errors = null)
        => new(false, default, errorType, message, errors);

}
