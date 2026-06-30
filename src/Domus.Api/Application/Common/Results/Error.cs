namespace Domus.Application.Common.Results;

public sealed record Error(string Code, string Message, int StatusCode = 400)
{
    public static Error Validation(string message) =>
        new("ERR_VALIDATION", message, StatusCodes.Status400BadRequest);

    public static Error NotFound(string code, string message) =>
        new(code, message, StatusCodes.Status404NotFound);

    public static Error Forbidden(string code, string message) =>
        new(code, message, StatusCodes.Status403Forbidden);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, StatusCodes.Status401Unauthorized);

    public static Error Conflict(string code, string message) =>
        new(code, message, StatusCodes.Status409Conflict);
}