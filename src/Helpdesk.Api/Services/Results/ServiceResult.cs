using System.Diagnostics.CodeAnalysis;

namespace HelpdeskTracker.Api.Services.Results;

public class ServiceResult
{
    private ServiceResult() { }

    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; private set; }
    public ErrorKind? Error { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static ServiceResult Ok()
    {
        return new ServiceResult
        {
            IsSuccess = true
        };
    }

    public static ServiceResult Fail(ErrorKind errorKind, string errorMessage)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            Error = errorKind,
            ErrorMessage = errorMessage
        };
    }
}