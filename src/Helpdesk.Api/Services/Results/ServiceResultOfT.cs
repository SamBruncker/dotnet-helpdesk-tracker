using System.Diagnostics.CodeAnalysis;

namespace HelpdeskTracker.Api.Services.Results;

public class ServiceResult<T>
{
    private ServiceResult() { }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public ErrorKind? Error { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static ServiceResult<T> Ok(T value)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Value = value
        };
    }

    public static ServiceResult<T> Fail(ErrorKind errorKind, string errorMessage)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Error = errorKind,
            ErrorMessage = errorMessage
        };
    }
}